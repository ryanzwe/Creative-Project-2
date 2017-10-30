#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "common.h"
#include "commonPlayer.h"
#include "spoiler.h"

int your_turn(GameState* gameState) {
    //if last actions werent s or l
    int x = gameState->players[gameState->currentPlayer].x;
    int y = gameState->players[gameState->currentPlayer].y;
    if ((gameState->lastAction != 's') && (gameState->lastAction != 'l')) {
        //check for short range target and select this move if one is found
        for (int i = 0; i < gameState->totalPlayers; i++) {
            if ((x == gameState->players[i].x) && (y == gameState->players
                    [i].y) && (i != gameState->currentPlayer)) {
                gameState->lastAction = 's';
                printf("plays\n");
                fflush(stdout);
                return 0;
            }
        } //check for long range target and select this move if one is found
        for (int i = 0; i < gameState->totalPlayers; i++) {
            int otherY = gameState->players[i].y;
            int otherX = gameState->players[i].x;
            if ((((y == 0) && (otherY == 0) && ((otherX == x + 1) || 
                    (otherX == x - 1)) && (x != otherX)) || ((y == 1) && 
                    (otherY == 1) && (x != otherX))) && (i != gameState->
                    currentPlayer)) {
                gameState->lastAction = 'l';
                printf("playl\n");
                fflush(stdout);
                return 0;
            }
        }             
    } //if there is a player above or below move to them
    for (int i = 0; i < gameState->totalPlayers; i++) {
        int otherY = gameState->players[i].y;
        int otherX = gameState->players[i].x;
        if ((x == otherX) && (((y + 1) % 2) == otherY)) {
            gameState->lastAction = 'v';
            printf("playv\n");
            fflush(stdout);
            return 0;
        }
    } //if there is loot declare to pick it up
    if (gameState->loot[x][y] > 0) {
        gameState->lastAction = '$';
        printf("play$\n");
        fflush(stdout);
        return 0;
    } //by default move horizontally
    gameState->lastAction = 'h';
    printf("playh\n");
    fflush(stdout);
    return 0;        
}

int move_where(GameState* gameState) {
    int x = gameState->players[gameState->currentPlayer].x;
    char direction;
    int left = 0;
    int right = 0;
    //move int the direction of the most players
    for (int i = 0; i < gameState->totalPlayers; i++) {
        int otherX = gameState->players[i].x;
        if (otherX < x) {
            left++;
        } else if (otherX > x) {
            right++;
        }
    }
    if (left > right) {
        direction = '-';
    } else if (right > left) {
        direction = '+';
    } else if (x != 0) {
        direction = '-';
    } else {
        direction = '+';
    }
    printf("sideways%c\n", direction);
    fflush(stdout);
    return 0;
}

int short_target(GameState* gameState) {
    int x = gameState->players[gameState->currentPlayer].x;
    int y = gameState->players[gameState->currentPlayer].y;
    int target = -1;
    //search for valid short targets
    for (int i = 0; i < gameState->totalPlayers; i++) {
        if ((x == gameState->players[i].x) && (y == gameState->players[i].y) &&
                (i != gameState->currentPlayer)) {
            target = i;
        }
    }
    //if target is found declare it
    if (target == -1) {
        printf("target_short-\n");
    } else {
        printf("target_short%c\n", ntoc(target));
    }
    fflush(stdout);
    return 0;
}

int long_target(GameState* gameState) {
    int x = gameState->players[gameState->currentPlayer].x;
    int y = gameState->players[gameState->currentPlayer].y;
    int target = -1;
    //search for valid long targets
    for (int i = 0; i < gameState->totalPlayers; i++) {
        int otherY = gameState->players[i].y;
        int otherX = gameState->players[i].x;
        if ((((y == 0) && (otherY == 0) && ((otherX == x + 1) || (otherX == x -
                1)) && (x != otherX)) || ((y == 1) && (otherY == 1) && (x !=
                otherX))) && (i != gameState->currentPlayer)) {
            target = i;
        }
    }
    //if one is found declare it
    if (target == -1) {
        printf("target_long-\n");
    } else {
        printf("target_long%c\n", ntoc(target));
    }
    fflush(stdout);
    return 0;
}

int execute_phase(GameState* gameState, char* input) {
    int returned;
    if (strcmp(input, "h?\n") == 0) {
	move_where(gameState);
    } else if (strcmp(input, "s?\n") == 0) {
	short_target(gameState);
    } else if (strcmp(input, "l?\n") == 0) {
	long_target(gameState);
    } else if ((strncmp(input, "hmove", 5) == 0) && 
	    (strlen(input) == 8)) {
	returned = hmove(gameState, input); 
    } else if ((strncmp(input, "vmove", 5) == 0) && 
	    (strlen(input) == 7)) { 
	returned = vmove(gameState, input); 
    } else if ((strncmp(input, "long", 4) == 0) && 
	    (strlen(input) == 7)) {
	returned = long_function(gameState, input);
    } else if ((strncmp(input, "short", 5) == 0) && 
	    (strlen(input) == 8)) {
	returned = short_function(gameState, input);
    } else if ((strncmp(input, "looted", 6) == 0) && 
	    (strlen(input) == 8)) {
	returned = looted(gameState, input);
    } else if ((strncmp(input, "driedout", 8) == 0) &&
	    (strlen(input) == 10)) {
        returned = driedout(gameState, input);
    } else {
	return 6;
    }
    return returned;
}

int main_loop(GameState* gameState) {
    char input[12];
    int phase = 2; //starting variable to make sure round is first
    while(1) {
        if (fgets(input, 12, stdin) != NULL) {
            int returned = 0;
            //game over
            if (strcmp(input, "game_over\n") == 0) {
                return 0;
            //swap phase
            } else if (strcmp(input, "round\n") == 0) {
                if (phase == 0) {
                    return 6;
                }
                phase = 0;
            } else if (strcmp(input, "execute\n") == 0) {
                if (phase == 1 || phase == 2) {
                    return 6;
                }
                phase = 1;
            //orders phase
            } else if (phase == 0) { 
                if (strcmp(input, "yourturn\n") == 0) {
                    your_turn(gameState);
                } else if ((strncmp(input, "ordered", 7) == 0) && 
                        (strlen(input) == 10)) {
                    returned = ordered(gameState, input);
                } else {
                    return 6;
                }
            //execute phase
            } else if (phase == 1) {
                returned = execute_phase(gameState, input);
            //below are errors
            } else {
                return 6;
            }
            if (returned == 6) {
                return 6;
            }
        } else {
            return 6;
        }
    }
    return 6;
}

int main(int argc, char** argv) {
    if (argc != 5) {
        fprintf(stderr, "Usage: player pcount myid width seed\n");
        return 1;
    }
    //initialise GameState struct with input arguments if they are valid
    GameState gameState;
    char buffer[50];
    int num = sscanf(argv[1], "%d%s", &gameState.totalPlayers, buffer);
    if ((num != 1) || (gameState.totalPlayers < 1) || (gameState.totalPlayers
            > 26)) {
        fprintf(stderr, "Invalid player count\n");
        return 2;
    } 
    num = sscanf(argv[2], "%d%s", &gameState.currentPlayer, buffer);
    if ((num != 1) || (gameState.currentPlayer > gameState.totalPlayers - 1) ||
            (gameState.currentPlayer < 0)) {
        fprintf(stderr, "Invalid player ID\n");
        return 3;
    }
    num = sscanf(argv[3], "%d%s", &gameState.carriages, buffer);
    if ((num != 1) || (gameState.carriages < 3)) {
        fprintf(stderr, "Invalid width\n");
        return 4;
    }
    num = sscanf(argv[4], "%d%s", &gameState.seed, buffer);
    if ((num != 1) || (gameState.seed < 0)) {
        fprintf(stderr, "Invalid seed\n");
        return 5;
    }
    //allocate memory to struct and print initial !
    setup(&gameState);
    char* out = "!\0";
    printf("%s", out);
    fflush(stdout);
    //run main game and check for proper exit status
    int returnValue = main_loop(&gameState);
    if (returnValue == 0) {
        free_game_state(&gameState);
        return 0;
    }
    //if no proper exit return with error
    fprintf(stderr, "Communication Error\n");
    free_game_state(&gameState);
    return 6;
 

}
