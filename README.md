# Intersect

- Based on the [Chuck (TV Series)](https://en.wikipedia.org/wiki/Chuck_(TV_series)).
The program is the representation of the [Orion computer's password terminal]().


Preview:

![](https://i.imgur.com/WL8IOA3.png)

# Setup (How to use):

When running the program it will create a folder called System Files with the necessary files for the program to work correctly in the same directory where the executable is.

In this directory are found the following files:
- **Intersect (Folder):**
In this folder you can place your Intersect image files.
Images of the following types will be accepted: `.png .gif .jpg .bmp .tiff`

	Notes:
> *- If there are no images in the folder, the program will still work, but nothing will be displayed.*

> *- Images will be displayed very quickly by default, so a large number of images is needed to get the desired result.*

> *- Images will be read in milliseconds, you should put the images in low resolution.*

- **conf.json:**
Here you can configure the behavior of your program and how it will work.

	**Intersect Image Delay:** It is the time in ms that each image will be displayed.
	**Intersect Message:** Message that will be shown after loads the Intersect.
	**Welcome Message:** Message that will be shown before loads the Intersect.
	**Clipboard:** Text that will be copied to the clipboard after loads the Intersect.
	**Skip:** Possibility to skip all questions by typing the answer of the last question in the first.
	**Debug:** If the program throws an exception, it will be copied to the clipboard.

```json
{
	"Intersect Image Delay": 50, 
	"Intersect Message": "Activation Complete.", 
	"Welcome Message": "Hello Son.", 
	"Clipboard": "You're Special, Son.", 
	"Skip": true, 
	"Debug": true
}
```

- **data.json:** 
	It will allow you to create your own quiz in the program with your questions and your answers.

```json
[
	{"Question": "Knock Knock.", "Answer": "I'm Here."}, 
	{"Question": "1 or 11 ?", "Answer": "Aces, Charles."}
]
```
