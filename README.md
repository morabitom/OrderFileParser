# Order File Parsing Application

## Approach

I decided to use the "smart constructor" pattern with the main data classes, I thought this would be a good fit because
the construction logic is a little bit more complicated than a simple initialization, we have to check for errors and log
them to the parent object, loop through and parse sub records, etc. I'm not dead set on this approach, it just seemed
like a decent fit.

The first step I took was simplifying the raw data we were getting from the file by presenting the orders through an 
IEnumerable that allows the calling code to loop over the raw data lines grouped by order. I'm not too happy with the code
that does this (GetRawOrdersData), it's a bit difficult to understand at a glance. If I were to give myself more time on
the project I would create a class inheriting from List\<string\> to represent the raw data and move the enumeration logic 
there, to make the intention of the code more obvious. 

I thought I was being clever by implementing a generic parser for individual raw data fields, but then realized bool.TryParse()
does not support "0" and "1", only "true" and "false". As a result I spent extra time there I didn't have to accounting
for that edge case. I would have been done sooner if I had skipped the generic approach.

## Ideas for improvement

If I had more time on the project I would first refactor the DataFileUtilities into a class inheriting from IEnumerable<string>
called RawOrderFileData. I feel it would be more intention revealing and a natural place for mapping the RowType enum.

Another thing I would consider is refactoring the smaller classes into records. The simple data layout seems to call for 
it, and I think it would remove some lines of code from those files while only adding features to the objects themselves.

Also, there is probably a better approach to parsing the data lines instead of passing the raw starting indexes, since 
the indexes and lengths are cumulative, it may be possible to simplify the code by associating a field with only its 
length. This would make the parser more extensible, new fields could be added to the data file format and the starting 
indexes of the fields wouldn't need to be manually updated.

I would also go through and make the rest of the code async, I'm sure there are many places where async versions of 
framework methods exist, but I used the synchronous version instead.

Lastly I would ask if there was any flexibility in the way errors are expected to be logged. Logging errors in the 
individual Order objects was cumbersome and resulted it me passing the Errors property through the class hierarchy. This
felt like a pretty bad code smell to me due to it working through side effects and appearing all over the code.I would 
have preferred to log the errors to a singleton instance. It's also possible I was taking the final sentence of the 
instructions too literally and logging the errors necessarily on the Order object wasn't a requirement.

