We experienced a strange behaviour: cl.exe isn't able to compile a simple test program while configuring with CMake. Here's a solution to this problem:

"I would like to point out to other complete noobs like myself however that if
   $(SystemRoot)
   $(SystemRoot)\System32
   $(SystemRoot)\System32\wbem

are not in your 'Show directories for: Executable files' list in your Tools > Options > Projects and Solutions > VC++ Directories tab you have to ADD them to the list and then use the arrow button to move them up above the $(PATH) variable. (Duh!)"

(Source: http://social.msdn.microsoft.com/Forums/en-US/vcgeneral/thread/be69a2fe-df55-432b-8c53-9edd10e1d745/)