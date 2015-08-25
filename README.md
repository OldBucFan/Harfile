# Harfile
Initial GitHub push - 
Google Chrome's Developer Console (F12) has a network capture tab. When activated, the capture catches the request 
and response for everything in the target Chrome tab. Right-click any item in the displayed list of requests and you
may select the "Save as HAR with Content" option to download the entire capture as a .har file. This is essentially
a JSON formatted file witht the details of the requests and responses contained within.

What the Harfile application does is reads the .har file, then summarizes timings and errors. Not incredibly cool at this 
point, but it is helpful to quickly review performance of a web application, check for backend GET/POST requests 
that error, and identify slow transactions.

I'd like to improve the output display and change from a rahter static dataGridView to a table that can be sorted on
any column. It would also be nice to have a drill-down available from each summary line.
