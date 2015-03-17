# Introduction #

## Targets ##
Our main goal is to classify facial expressions within video streams. This implies the following sub-targets:

### Face detection ###
Currently we use cascade of Haar-based classifiers to detect a face and facial regions within a face (a modified Voila-Jones approach).

### Facial region determination ###
For rough determination of facial regions we use anthropometric proportions.

### Facial feature extraction ###
Detection of the individual facial features are a matter of research at the moment. We currently experiment with iterative thresholds.

### Classification of facial expression ###
After the extraction of facial features we use an artifical neural network to train facial expressions using backpropagation or rprop.

## Tools ##
OpenCV is the central piece of software for all the Computer Vision and Computation Intelligence related programming. For the UI we tried out WPF and C# which is a great way to program but the whole mashalling between iur C library and C# is a definitely not good at the moment.

# Image processing #

The following table illustrates how an input image will be processed to find facial feature points (FFPs). Read it from left to right and top to bottom.

<table cellpadding='1' border='1' cellspacing='0'>
<tr>
<th>1. Input Image</th>
<th>2. Mixed processing</th>
<th>3. Extract ROI</th>
<th>4. Convert to Grayscale</th>
<th>5. Stretch contrast from "upper boundary" of the grayscale distribution to 255</th>
<th>6. Stretch contrast from 0 to "lower boundary" of the grayscale distribution</th>
<th>7. Inverse Binarization</th>
<th>8. Find contour</th>
<th>9. Mark feature points in contour with white crosses</th>
<th>10. Final image with Feature Points</th>
</tr>
<tr>
<td>
<img src='http://klucv2.googlecode.com/svn/trunk/data/gesicht_mann.jpg' />
</td>
<td>
<ol>
<li>Find the face in the image using a cascade of Haar-based classifiers.<br />
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100217-1/ffp_face.png' />
</li>
<li>Subdivide face into estimated facial feature regions somehow based on anthropometric measurement (relative distances based on distance of the eyes). After that we do a search for facial features like eyes and mouth, in these estimated facial feature regions using different cascades of Haar-based classifiers.<br />
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100217-1/ffp_search_regions.png' /></li>
</ol>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re1.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re2.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re3.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re4.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re5.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re6.png' />
<br />Right eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/re7.png' />
<br />Right eye<br>
</td>
<td>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100217-1/ffp_ffp.png' />
</td>
</tr>
<tr>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le1.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le2.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le3.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le4.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le5.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le6.png' />
<br />Left eye<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/le7.png' />
<br />Left eye<br>
</td>
<tr>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m1.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m2.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m3.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m4.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m5.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m6.png' />
<br />Mouth<br>
</td>
<td align='center'>
<img src='http://klucv2.googlecode.com/svn/trunk/data/result-images/20100323-1/m7.png' />
<br />Mouth<br>
</td>
</tr>
</tr>
</table>

## Demo ##
There'll be a ready to use proof-of-concept program in the near future.

Have fun!