# What is it?
Constellation is a node based programming language for unity that gives you the tools of a programmer without having to write a single line of code. 
![alt text](https://static.wixstatic.com/media/cbe6c9_c583619449df44bbb2a89427973123a3~mv2.png/v1/fill/w_897,h_715,al_c,usm_0.66_1.00_0.01/cbe6c9_c583619449df44bbb2a89427973123a3~mv2.png)

# Who should use it?
Designers, Artiste, programmers... Constellation is easy to use and extendable if you want to use your custom scripts.

# Features
- Editor: Beta.
- Live editing: preview (very buggy).
- Examples: in development.
- Basic nodes to do a game.

# Quick start
- Download or clone the repo.
- Open with unity 2017.3.
- Open the constellation Window.

# Anatomy of a node
![alt text](https://static.wixstatic.com/media/cbe6c9_908c53aaea714a2e8c80f5515578e157~mv2.png/v1/fill/w_600,h_233,al_c,usm_0.66_1.00_0.01/cbe6c9_908c53aaea714a2e8c80f5515578e157~mv2.png)
- Inputs set variables in the node.
- Output are the result.
- A warm input will trigger an output.
- A warm output is triggered without warm input. For example keyDown node will trigger an output because a key is pressed.
- A cold output is triggered only when a warm input receive a message.
- A cold input does not trigger an output.
- A Variable is either a number, a string, an array, an object, or a list of variable (Very similar to the var in javascript).
- A Unity variable is an object wich inherite from unity object.
- Links alow you to connect a node output to a node input.

