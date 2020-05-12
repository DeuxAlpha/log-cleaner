# Introduction

This project is a console application intended to clean log files for X4
Foundations but can be used for any kind of file, really.

# Disclaimer

The primary intention for this program was to clean log files from X4
Foundations, because it creates a bunch of errors due to missing .sig
files.

If this is why you came here, I would encourage you to check out the [X4
Customizer](https://forum.egosoft.com/viewtopic.php?t=405275), which has
a script to modify the X4 executable to not create those messages in the
first place.  
However, since I am incapable of figuring out how to run a simple script
to get that process to work on my system, I wrote this program instead.

# Whatever you say, dude. So, how does it work?

Basically, the program watches for the log files wherever you tell it
to, copies it to a temporary file (since the log file is actively being
written to by X4 and you don't/can't want to mess with that), removes
all the stuff you don't want based on filters, and writes it to a new
file.

# Program options

You may run the program with various options. Simply run
`Cleaner-- help` to see them.

The easiest way to get the program to do what you want would be to set
up a script with some default parameters. Mine looks like this:

```bash
Cleaner -d ../ --filter-directory ../Filtered --debug
```
