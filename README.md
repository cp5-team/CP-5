# CP-5 Emulator v1.2

CP-5 is a lightweight 8-bit virtual processor emulator written in C#. It features a custom instruction set designed for low-level logic experimentation, terminal-based games, and simple automation scripts.

## Core Features
- Dynamic memory allocation via CLI arguments.
- Label-based navigation and conditional branching (IF/GOTO).
- Built-in file system operations (Reading/Writing).
- Direct memory pointer manipulation.
- Console-based I/O with character and numeric support.

## Installation
1. Download the latest `cp5.exe`.
2. Move it to a folder of your choice.
3. Add that folder to your System PATH to use the `cp5` command globally.

## Command Line Usage
- Run a script: `cp5 program.cp5`
- Set memory size: `cp5 program.cp5 -mem 1024`
- Set starting pointer: `cp5 program.cp5 -sp 10`
- View help: `cp5 --help`

## Instruction Set Overview

MEMORY
V[value] - Set current cell value (0-255).
JP [pos] - Move memory pointer to a specific address.
B        - Move pointer forward by 1.
C        - Move pointer backward by 1.
CLM      - Reset all memory cells to zero.

I/O
1        - Print current cell as ASCII character.
F        - Print current cell as numeric value.
LK       - Read user input string into memory.
P        - Play a system beep.
CLS      - Clear the console screen.
0        - Pause and wait for any key.

LOGIC
: [name] - Define a jump label.
G [name] - Jump to a label or line number.
IF [a] [op] [b] : [cmd] - Conditional jump. Operators: EQL, BG, BL.
!:       - Terminate the program.

FILES
WF [file] [text] - Write text to a file.
RF [file]        - Read and display file content.
RFTM [file]      - Load binary file data into memory.

## Example Script (Counter)
: START
V65 1    // Sets cell to 65 ('A') and prints it
4        // Increment value
IF 0 BL 90 : G START // If value < 90, jump to START
!:       // Exit

## License
This project is licensed under the MIT License.
