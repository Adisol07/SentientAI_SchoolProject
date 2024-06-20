#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include "dictionary.h"
#include "io.h"

typedef struct
{
    char *word;
    float value;
} WordValue;

static WordValue *list = NULL;
static int numWords = 0;

bool WordExists(char *word)
{
    for (int i = 0; i < numWords; i++)
    {
        if (strcmp(word, list[i].word) == 0)
        {
            return true;
        }
    }
    return false;
}

void AddWord(char *word, float value)
{
    if (WordExists(word))
    {
        fprintf(stderr, "Word '%s' already exists\n", word);
        return;
    }

    list = (WordValue *)realloc(list, (numWords + 1) * sizeof(WordValue));
    if (list == NULL)
    {
        fprintf(stderr, "Memory allocation failed\n");
        return;
    }

    list[numWords].word = (char *)malloc(strlen(word) + 1);
    if (list[numWords].word == NULL)
    {
        fprintf(stderr, "Memory allocation failed\n");
        return;
    }
    strcpy(list[numWords].word, word);

    list[numWords].value = value;

    numWords++;
}

float GetValue(char *word)
{
    for (int i = 0; i < numWords; i++)
    {
        if (strcmp(word, list[i].word) == 0)
        {
            return list[i].value;
        }
    }

    return -1;
}

void SetValue(char *word, float value)
{
    for (int i = 0; i < numWords; i++)
    {
        if (strcmp(word, list[i].word) == 0)
        {
            list[i].value = value;
        }
    }
}

void PrintDictionary()
{
    printf("Dictionary content: \n");
    for (int i = 0; i < numWords; i++)
    {
        printf("%s : %.2f\n", list[i].word, list[i].value);
    }
}
void Cleanup()
{
    if (list != NULL)
    {
        for (int i = 0; i < numWords; i++)
        {
            free(list[i].word);
        }
        free(list);
    }
}