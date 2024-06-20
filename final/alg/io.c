#include <stdio.h>
#include <stdlib.h>
#include "io.h"

char *ReadFile(char *path)
{
    FILE *file;
    char *content = NULL;
    long length = 0;

    file = fopen(path, "r");
    if (file == NULL)
    {
        perror("Error while opening text file");
    }

    fseek(file, 0, SEEK_END);
    length = ftell(file);
    fseek(file, 0, SEEK_SET);

    content = (char *)malloc(length + 1);
    if (content == NULL)
    {
        perror("Memory allocation failed");
        fclose(file);
        return NULL;
    }

    fread(content, 1, length, file);
    
    content[length] = '\0';
    
    fclose(file);
    
    return content;
}