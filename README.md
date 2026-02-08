# Order File Parsing Application

## Approach

After taking a look at how StreamReader is implemented I decided to used Span\<T\> to avoid creating new strings when parsing
the individual elements of each text file line. This span is passed by ref to the appropriate parsing method. I used structs 
to construct the individual Order object elements for a small performance increase.

## Ideas for improvement

To avoid creating a new string for every line of the text file, I could read each line into a buffer and then access that 
buffer as a Span.

Also, there is probably a better approach to parsing the data lines instead of passing the raw starting indexes, since 
the indexes and lengths are cumulative, it may be possible to simplify the code by associating a field with only its 
length. This would make the parser more extensible, new fields could be added to the data file format and the starting 
indexes of data element wouldn't need to be manually updated.

