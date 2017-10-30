#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "common.h"
#include "commonPlayer.h"
#include "acrophobe.h"

int your_turn(GameState* gameState) {
    //if there is no loot move
    if (gameState->loot[gameState->players[gameState->currentPlayer].x]
            [gameState->players[gameState->currentPlayer].y] == 0) {
        printf("playh\n");
        fflush(stdout);
    } else { //otherwise loot
        printf("play$\n");
        fflush(stdout);
    }
    return 0;        
}

int move_where(GameState* gameState) {
    //move in last direction unless at edge of carriages
    int currentX = gameState->players[gameState->currentPlayer].x;
    if (currentX == 0) {
        printf("sideways+\n");
        fflush(stdout);
    } else if (currentX == gameState->carriages - 1) {
        printf("sideways-\n");
        fflush(stdout);
    } else {
        printf("sideways%c\n", gameState->lastMove);
        fflush(stdout);
    }
    return 0;
}

int execute_phase(GameState* gameState, char* input) {
    int returned;
    //check for matching input
    if (strcmp(input, "h?\n") == 0) {
	move_where(gameState);
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
            //end game
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
            } else { //invalid input
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
    //setup GameState if inputs are valid
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
    //allocate memory and print initial !
    setup(&gameState);
    char* out = "!\0";
    printf("%s", out);
    fflush(stdout);
    //main game loop, check for normal exit
    int returnValue = main_loop(&gameState);
    if (returnValue == 0) {
        free_game_state(&gameState);
        return 0;
    }
    //no normal exit, return error
    fprintf(stderr, "Communication Error\n");
    free_game_state(&gameState);
    return 6;
}
