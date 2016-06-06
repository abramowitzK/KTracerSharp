Usage:

make run ARGS=100

Assignment 3 Update:

This will build and run the program with the max triangles per bounding sphere set to 100.

I found 100 to be a good number to use. 1000 will double the time for rendering but any lower than 50 causes issues.
The default without entering any argument is 100

I used a bounding volume heirarchy with bounding spheres. The don't fit as well but the intersection testing is very fast.

website: cs.drexel.edu/~kra46/cs431.html

The code for creating the BVH is in Scene.cs. The boundingSphere.cs file also has the bounding sphere related stuff. 
Finally the tracing is done in Ray.cs where we go through the heirarchy.

I was able to render the 69k face bunny in about 40 seconds (with other meshes) at 2048x2048. 
This previously took over 20 minutes at 1024x1024 so this was a huge speedup. Almost 100x faster when you consider the difference in resolution.

Assignment 4 Update:

Using 5% tolerance. Most of the code for implementing the Supersampling AA is in Camera.cs.

website: cs.drexel.edu/~kra46/cs431.html

Used the 1k bunny, the 5k cow and icosohedron and 4 spheres. There are 2 white lights in the same position as assignment 2 and 3

For the intensity image. I started with a base color of (0.05,0.05,0.05,1.0).
Then multiplied the number of rays per pixel by this color.


5/26/2016
Assignment 5 Update:

used same SSAA code from hw 4 with same parameters.

website: cs.drexel.edu/~kra46/cs431.html

The code implementing shadows and also the code that implements reflections.
Shadows:
Starting at line 139
Reflections:
Starting at line 160

There scene is assembled and parameters assigned in Program.cs. 

6/26/2016
Assignment 6 Update:

website cs.drexel.edu/~kra46/cs431.html

The code implementing refractions is in Ray.cs in the Trace function

Starting at line 176 and ending at line 194
Program.cs contains the code that sets up all the objects with materials and transforms.

There is one white light at (0,10,20) with 400 intensity.

There is a purely refractive sphere which you can see the cow and bunny through as well as a purely refractive icosohedron.

The bunny is blue with .4 kd and .6 ks and 100 as it's specular exponent

The cow is red with .1 ka, .3 kd, .6 ks and 100 specular exponent

the background is a scaled box with a green color 150 specular exponent .15 ka, .4 kd .6 ks
