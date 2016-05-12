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