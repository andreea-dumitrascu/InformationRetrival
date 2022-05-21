# InformationRetrival
This is a school project witch purpouse is to make a program that can search into XML file some information given into an input.
This is a C# .Net project

### Pre-requirements
1. If you don't have some libreries included into yout IDE, you need to install some nuGets
 * System.Xml.XDocument
2. In folder

### Implementation
#### Article class
This class was implemented for keepeng the record of all articles read from the files and the prealucration made on them. 
In this article I extracted all words, removig all wight spaces and puctuation, and i mad all the words lowercase. I also have a file with stop words used for removing thes words from articles. I used PorterStemmer class for extracting the child of an word. I also have to mention that in my implemenation i removed all numbers and years, or dates.
I also made an apparitionDictiaonary used to make the rare matrix. I used this to make an output file named "out.txt" (see the main of the program) that it is an (ARFF)[https://www.cs.waikato.ac.nz/~ml/weka/arff.html].
The normalizedDictionary it is used to keep the record of the normalization of words. For this phase I used the nominal normalization by this formula:
