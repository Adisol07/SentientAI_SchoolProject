#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include "dictionary.h"
#include "io.h"

#define MAX_INPUT_SIZE 512

char *TrainingFiles[] = {
    "../../data/compiled/trainData1.txt",
    "../../data/compiled/trainData2.txt",
    "../../data/compiled/trainData3.txt",
    "../../data/compiled/trainData4.txt",
    "../../data/compiled/trainData5.txt",
    "../../data/compiled/trainData6.txt",
    "../../data/compiled/trainData7.txt",
    "../../data/compiled/trainData8.txt",
    "../../data/compiled/trainData9.txt",
    "../../data/compiled/trainData10.txt",
};

void ToLower(char *str)
{
    int i = 0;
    while (str[i])
    {
        str[i] = tolower(str[i]);
        i++;
    }
}

void ToDictionary(char *comment, int score)
{
    const char *delimiters = " \t\n";

    char *word;
    word = strtok(comment, delimiters);

    while (word != NULL)
    {
        if (WordExists(word))
        {
            float value = (GetValue(word) + score) / 2;

            SetValue(word, value);
        }
        else
        {
            AddWord(word, score);
        }

        word = strtok(NULL, delimiters);
    }
}
void Train()
{
    printf("Training.. \n");

    const char splitChar = ';';
    for (int i = 0; i < sizeof(TrainingFiles) / sizeof(TrainingFiles[0]); i++)
    {
        printf("Training from %s\n", TrainingFiles[i]);

        char *fileContent = ReadFile(TrainingFiles[i]);

        char *line = (char *)malloc(1);
        for (int j = 0; fileContent[j] != '\0'; j++)
        {
            if (fileContent[j] == '\n')
            {
                char *comment;
                int score;

                comment = strtok(line, &splitChar);
                score = atoi(strtok(NULL, &splitChar));

                ToDictionary(comment, score);

                strcpy(line, "");
            }
            else
            {
                char *str;
                int len = strlen(line);
                str = (char *)realloc(line, (len + 2) * sizeof(char));

                str[len] = fileContent[j];
                str[len + 1] = '\0';

                line = str;
            }
        }

        free(fileContent);
    }

    printf("Training done\n");
}

int main()
{
    Train();
    printf("\n");

    while (true)
    {
        printf("Sentence: ");
        char input[MAX_INPUT_SIZE];
        fgets(input, MAX_INPUT_SIZE, stdin);
        if (input[strlen(input) - 1] == '\n')
        {
            input[strlen(input) - 1] = '\0';
        }

        if (strcmp(input, "$PRINTALL") == 0)
        {
            PrintDictionary();
            continue;
        }
        else if (strcmp(input, "$TRAIN") == 0)
        {
            Train();
            continue;
        }

        ToLower(input);

        const char *delimiters = " \t";
        char *word;
        word = strtok(input, delimiters);

        int value = 0;
        int count = 0;
        while (word != NULL)
        {
            if (WordExists(word))
            {
                value += GetValue(word);
            }
            else
            {
                printf("Word %s is not present in word dictionary!\n", word);
                value += 50;
            }

            count++;
            word = strtok(NULL, delimiters);
        }

        float score = (float)value / (float)count;
        printf("Score: %.2f\n", score);
    }

    return 0;
}