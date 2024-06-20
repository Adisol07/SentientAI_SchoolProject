#ifndef DICTIONARY

#include "io.h"

#define DICTIONARY

bool WordExists(char *word);
void AddWord(char *word, float value);
float GetValue(char *word);
void SetValue(char *word, float value);
void PrintDictionary();
void Cleanup();

#endif