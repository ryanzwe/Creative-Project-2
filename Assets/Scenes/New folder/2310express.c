#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <signal.h>
#include <unistd.h>
#include <sys/wait.h>
#include "common.h"
#include "2310express.h"

/*
 * Global variable that is set by the signal handler that keeps track of if
 * the game should be closed, it is 1 if there was a client disconnection
 */
int disconnected = 3;

void disconnect_handler(int sig) {
    if (sig == SIGINT) {
        fprintf(stderr, "SIGINT caught\n");
        exit(2);
    } else if (disconnected == 0) { 
	pid_t pid;
        int status;
        while((pid = waitpid((pid_t) (-1), &status, WNOHANG)) > 0) {
            if (status != 0) {
                fprintf(stderr, "Player %d ended with status %d\n", pid, 
                        status);
            }
            disconnected = 1;
        }
    }
}

int setup_pipes(GameState* gameState, PipeInfo* pipeInfo) {
    pipeInfo->toPlayer = calloc(gameState->totalPlayers, sizeof(int*));
    pipeInfo->fromPlayer = calloc(gameState->totalPlayers, sizeof(int*));
    pipeInfo->errorPlayer = calloc(gameState->totalPlayers, sizeof(int*));

    for (int i = 0; i < gameState->totalPlayers; i++) {
        pipeInfo->toPlayer[i] = calloc(2, sizeof(int));
        pipeInfo->fromPlayer[i] = calloc(2, sizeof(int));
        pipeInfo->errorPlayer[i] = calloc(2, sizeof(int));
    }
    return 0;
}

int free_pipes(GameState* gameState, PipeInfo* pipeInfo) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        free(pipeInfo->toPlayer[i]);
        free(pipeInfo->fromPlayer[i]);
        free(pipeInfo->errorPlayer[i]);
    }
    free(pipeInfo->toPlayer);
    free(pipeInfo->fromPlayer);
    free(pipeInfo->errorPlayer);
    free_game_state(gameState);
    return 0;
}

int send_all(GameState* gameState, PipeInfo* pipeInfo, char* message) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        write(pipeInfo->toPlayer[i][1], message, strlen(message));
    }
    return 0;
}

int ask_moves(GameState* gameState, PipeInfo* pipeInfo, char* answers) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        //player needs to dry out - no move this turn
        if (gameState->players[i].hits >= 3) {
            answers[i] = 'd'; 
            continue;
        } 
        //ask for input and process
        char message[10] = "yourturn\n";
	write(pipeInfo->toPlayer[i][1], message, strlen(message));
        char fullAnswer[8] = {0};
	if (read(pipeInfo->fromPlayer[i][0], fullAnswer, 7) < 1) {
            return 4;
        }
        if ((strncmp(fullAnswer, "play", 4) != 0) || fullAnswer[5] != '\n' ||
                fullAnswer[6] != '\0' || strchr(VALID_INPUTS, fullAnswer[4]) 
                == NULL) {
            return 5;
        } 
        answers[i] = fullAnswer[4];
    }    
    return 0;
}

int looted(GameState* gameState, int player) {
    int x = gameState->players[player].x;
    int y = gameState->players[player].y;
    //check if there is loot to take, players will display error messages if 
    //there isn't
    if (gameState->loot[x][y] > 0) {
        gameState->players[player].loot++;
        gameState->loot[x][y]--;
    }
    return 0;
}

int hmove(GameState* gameState, PipeInfo* pipeInfo, int player, char* 
        response) {
    char fullResponse[11] = {0};
    //ask for a response
    char message[4] = "h?\n";
    write(pipeInfo->toPlayer[player][1], message, strlen(message));
    if (read(pipeInfo->fromPlayer[player][0], fullResponse, 11) < 1) {
        return 4; 
    }
    if ((strncmp(fullResponse, "sideways", 8) != 0) || (fullResponse[10] != 
            '\0') || (fullResponse[9] != '\n') || ((fullResponse[8] != '-') && 
            (fullResponse[8] != '+'))) {
        return 5;
    } else { //recieved a valid response, can use it
        *response = fullResponse[8];
    }
    
    //update GameState struct
    if ((*response == '+') && (gameState->players[player].x != gameState->
            carriages)) {
        gameState->players[player].x++;
    } else if ((*response == '-') && (gameState->players[player].x != 0)) {
        gameState->players[player].x--;
    }
    return 0;
}

int execute_long(GameState* gameState, int player, int playerTarget) {
    //store x and y for easier access
    int xs = gameState->players[player].x;
    int xt = gameState->players[playerTarget].x;
    int ys = gameState->players[player].y;
    int yt = gameState->players[playerTarget].y;
    //check if conditions are met for long move
    if (ys != yt) {
        return 1;
    } else if (ys == 0) {
        if ((xs == xt - 1) || (xs == xt + 1)) {
            gameState->players[playerTarget].hits++;
        } else {
            return 1;
        }
    } else if (ys == 1) {
        if (xs != xt) {
            gameState->players[playerTarget].hits++;
        } else {
            return 1;
        }
    } else { //invalid instruction
        return 5;
    } 
    return 0;
}

int long_ask(GameState* gameState, PipeInfo* pipeInfo, int player, char* 
        response) {
    char fullResponse[14] = {0};
    //ask for response
    char message[4] = "l?\n";
    write(pipeInfo->toPlayer[player][1], message, strlen(message));
    if (read(pipeInfo->fromPlayer[player][0], fullResponse, 14) < 1) {
        return 4;
    }
    //check response 
    if ((strncmp(fullResponse, "target_long", 11) != 0) || (fullResponse[13]
            != '\0') || (fullResponse[12] != '\n')) {
        return 5;
    }
    int playerTarget = (int) fullResponse[11];
    if (invalid_player(gameState, playerTarget) && (playerTarget != DASH)) {
        if (playerTarget > 90) { //not a valid letter
            return 5;
        } else { //not a player curently in the game
            return 6;
        }
    }
    if (fullResponse[11] == '-') {    
        *response = fullResponse[11];
        return 0;
    }
    playerTarget -= OFFSET;
    //execute response
    int status = 0;
    status = execute_long(gameState, player, playerTarget);
    if (status == 5) {
        return status;
    } else if (status) {
        return 6;
    } else {
        *response = fullResponse[11];
    }
    return 0;
}

int short_ask(GameState* gameState, PipeInfo* pipeInfo, int player, char*
        response) {
    char fullResponse[15] = {0};
    //ask for response
    char message[4] = "s?\n";
    write(pipeInfo->toPlayer[player][1], message, strlen(message));
    if (read(pipeInfo->fromPlayer[player][0], fullResponse, 15) < 1) {
        return 4;
    } 
    //check response
    if ((strncmp(fullResponse, "target_short", 12) != 0) || (fullResponse[14]
            != '\0') || (fullResponse[13] != '\n')) {
        return 5;
    }
    int playerTarget = (int) fullResponse[12];
    if (invalid_player(gameState, playerTarget) && (playerTarget != DASH)) {
        if (playerTarget > 90) { //not a valid letter
            return 5;
        } else { //not a player curently in the game
            return 6;
        }
    }
    if (fullResponse[12] == '-') {    
        *response = fullResponse[12];
        return 0;
    }
    playerTarget -= OFFSET;
    //execute response
    int xs = gameState->players[player].x;
    int xt = gameState->players[playerTarget].x;
    int ys = gameState->players[player].y;
    int yt = gameState->players[playerTarget].y;
    if ((xs == xt) && (ys == yt)) {
        if (gameState->players[playerTarget].loot > 0) {
            gameState->players[playerTarget].loot--;
            gameState->loot[xt][yt]++;
        }
        *response = fullResponse[12];
    } else {
        return 6;
    }
    return 0;
}

int execute_phase(GameState* gameState, PipeInfo* pipeInfo, char* answers) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        if (answers[i] == '$') { //if player wants to loot
            char loot[9];
            sprintf(loot, "looted%c\n", ntoc(i));
            send_all(gameState, pipeInfo, loot);
            looted(gameState, i);
        } else if (answers[i] == 'v') { //if player wants to move vertically
            char vmove[8];
            sprintf(vmove, "vmove%c\n", ntoc(i));
            send_all(gameState, pipeInfo, vmove);
            gameState->players[i].y = (gameState->players[i].y + 1) % 2;
        } else if (answers[i] == 'h') { //if player wants to hmove
            char response;
            int returnValue = hmove(gameState, pipeInfo, i, &response);
            if (returnValue) {
                return returnValue;
            } 
            char hmove[9];
            sprintf(hmove, "hmove%c%c\n", ntoc(i), response);
            send_all(gameState, pipeInfo, hmove);
        } else if (answers[i] == 'l') { //if player wants to long shoot
            char response;
            int returnValue = long_ask(gameState, pipeInfo, i, &response);
            if (returnValue) {
                return returnValue;
            }
            char longResponse[8];
            sprintf(longResponse, "long%c%c\n", ntoc(i), response);
            send_all(gameState, pipeInfo, longResponse);
        } else if (answers[i] == 's') { //if player wants to short shoot
            char response;
            int returnValue = short_ask(gameState, pipeInfo, i, &response);
            if (returnValue) {
                return returnValue;
            }
            char shortResponse[9];
            sprintf(shortResponse, "short%c%c\n", ntoc(i), response);
            send_all(gameState, pipeInfo, shortResponse); 
        } else if (answers[i] == 'd') { //if player needs to dry out
            char driedout[11];
            sprintf(driedout, "driedout%c\n", ntoc(i));
            send_all(gameState, pipeInfo, driedout);
            gameState->players[i].hits = 0;
        } else { //invalid response
            return 6;
        }
    } 
    return 0;  
}

int print_status(GameState* gameState) {
    //print player stats
    for (int i = 0; i < gameState->totalPlayers; i++) {
        printf("%c@(%d,%d): $=%d hits=%d\n", ntoc(i), gameState->players[i].x,
                gameState->players[i].y, gameState->players[i].loot, gameState
                ->players[i].hits);
    } 
    //print carriage stats
    for (int i = 0; i < gameState->carriages; i++) {
        printf("Carriage %d: $=%d : $=%d\n", i, gameState->loot[i][0], 
                gameState->loot[i][1]);
    }
    fflush(stdout);
    return 0;
}

int print_winners(GameState* gameState) {
    char* winners = malloc(gameState->totalPlayers * sizeof(char));
    int numberWinners = 0;
    int largest = 0; //smallest is 0
    //find the player with the most loot
    for (int i = 0; i < gameState->totalPlayers; i++) {
        if (gameState->players[i].loot > largest) {
            largest = gameState->players[i].loot;
        }
    } 
    //check if other players have the same amount of loot
    for (int i = 0; i < gameState->totalPlayers; i++) {
        if (gameState->players[i].loot == largest) {
            winners[numberWinners] = ntoc(i);
            numberWinners++;
        }
    }
    //print winners
    printf("Winner(s):%c", winners[0]);
    for (int i = 1; i < numberWinners; i++) {
        printf(",%c", winners[i]);
    }
    printf("\n");
    fflush(stdout);
    free(winners);
    return 0;
}

int game_loop(GameState* gameState, PipeInfo* pipeInfo) {
    for (int i = 0; i < 15; i++) {
        //start the round and ask for moves
        send_all(gameState, pipeInfo, "round\n");
        char* answers;
        answers = malloc(gameState->totalPlayers * sizeof(char));
        int returnValue = ask_moves(gameState, pipeInfo, answers);
        if (returnValue) { //there was an issue with the input
            free(answers);
            return returnValue;
        } 
        //send information about orders to all players
        for (int i = 0; i < gameState->totalPlayers; i++) {
            char order[11];
            if (answers[i] != 'd') {
                sprintf(order, "ordered%c%c\n", ntoc(i), answers[i]);
                send_all(gameState, pipeInfo, order);
            }
        } 
        //execute phase ask for clarification when needed
        send_all(gameState, pipeInfo, "execute\n");
        returnValue = execute_phase(gameState, pipeInfo, answers);
        if (returnValue) { //there was an issue with the input
            free(answers);
            return returnValue;
        }
        free(answers);
        //print stats at end of round
        print_status(gameState);
        if (disconnected == 1) { //check for disconnections
            fprintf(stderr, "Client disconnected\n");
            free_pipes(gameState, pipeInfo);
            exit(4);
        }
    }
    //send game over and print winners after 15 rounds
    send_all(gameState, pipeInfo, "game_over\n");
    print_winners(gameState);
    return 0;
}

int get_initial_signal(GameState* gameState, PipeInfo* pipeInfo) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        char firstLetter[2] = {0};
        //check for first ! and end if not found
        read(pipeInfo->fromPlayer[i][0], firstLetter, 1);
        if (firstLetter[0] != '!') {
            fprintf(stderr, "Bad start\n");
            free_pipes(gameState, pipeInfo);
            exit(3);
        } 
    } 
    //enable reporting on stopped children as all have been initialised
    disconnected = 0;
    return 0;
}

int setup_children(GameState* gameState, PipeInfo* pipeInfo, char** argv) {
    for (int i = 0; i < gameState->totalPlayers; i++) {
        if (pipe(pipeInfo->toPlayer[i]) || pipe(pipeInfo->fromPlayer[i]) ||
                pipe(pipeInfo->errorPlayer[i])) {
            free_pipes(gameState, pipeInfo);
            fprintf(stderr, "Bad start\n");
            exit(1);
        }
        int pid = fork();
        if (!pid) { //child
            close(pipeInfo->toPlayer[i][1]);
            close(pipeInfo->fromPlayer[i][0]);
            close(pipeInfo->errorPlayer[i][0]);
            dup2(pipeInfo->toPlayer[i][0], 0); //redirect stdin
            dup2(pipeInfo->fromPlayer[i][1], 1); //redirect stdout
            dup2(pipeInfo->errorPlayer[i][1], 2); //redirect stderr
            close(pipeInfo->toPlayer[i][0]);
            close(pipeInfo->fromPlayer[i][1]);
            close(pipeInfo->errorPlayer[i][1]);
            //convert numbers to command line arguments
            char totalPlayers[48] = {0};
            char playerNumber[48] = {0};
            char carriages[48] = {0};
            char seed[48] = {0};
            sprintf(totalPlayers, "%d", gameState->totalPlayers);
            sprintf(playerNumber, "%d", i);
            sprintf(carriages, "%d", gameState->carriages);
            sprintf(seed, "%d", gameState->seed);
            execlp(argv[i + 3], argv[i + 3], totalPlayers, playerNumber, 
                    carriages, seed, NULL); //execute player
            fprintf(stderr, "Bad start\n"); //exec failed
            exit(1); 
        } else if (pid > 0) { //parent
            close(pipeInfo->toPlayer[i][0]);
            close(pipeInfo->fromPlayer[i][1]);
            close(pipeInfo->errorPlayer[i][1]);
        } else {
            free_pipes(gameState, pipeInfo);
            fprintf(stderr, "Bad start\n");
            exit(1);
        }
    }
    get_initial_signal(gameState, pipeInfo);
    return 0;
}

int main(int argc, char** argv) {
    //setup signal handler
    struct sigaction sa;
    sa.sa_handler = &disconnect_handler;
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = SA_NOCLDSTOP | SA_RESTART;
    sigaction(SIGINT, &sa, 0);
    sigaction(SIGCHLD, &sa, 0);
    sigaction(SIGPIPE, &sa, 0);
    //catch invalid arguments
    if (argc < 5) {
        fprintf(stderr, "Usage: hub seed width player player [player ...]\n");
        return 1;
    }
    //setup GameState with arguments if they are valid
    GameState gameState;
    char buffer[50] = {0};
    int num = sscanf(argv[1], "%d%s", &gameState.seed, buffer);
    int returnValue = 1;
    if ((num != 1) || (gameState.seed < 0)) {
        returnValue = 0;
    }
    num = sscanf(argv[2], "%d%s", &gameState.carriages, buffer);
    if ((num != 1) || (gameState.carriages < 3)) {
        returnValue = 0;
    }
    if (!returnValue) {
        fprintf(stderr, "Bad argument\n");
        return 2;
    }
    gameState.totalPlayers = argc - 3;
    //allocate memory to structs
    setup(&gameState);
    PipeInfo pipeInfo;
    setup_pipes(&gameState, &pipeInfo);
    setup_children(&gameState, &pipeInfo, argv);
    //run the main loop of the game
    returnValue = game_loop(&gameState, &pipeInfo);
    if (returnValue == 6) { 
        fprintf(stderr, "Illegal move by client\n");
    } else if (returnValue == 5) {
        fprintf(stderr, "Protocol error by client\n");
    } else if (returnValue == 4) {
        fprintf(stderr, "Client disconnected\n");
    }
    fflush(stderr);
    free_pipes(&gameState, &pipeInfo);
    exit(returnValue);
}
