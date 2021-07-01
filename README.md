# DisklessAsyncStreams

An example of processing data without it touching the disk using the C# 8 language feature: async streams

It contains two projects:

1. A web app that serves up a single page /BigFile.  When consumers hit that page it generates a moderately sized but easily scalable 260K CSV file in an asynchronous manner simulating hitting a database.

2. A command line app that calls into /BigFile and writes out every 10 records but that does not save the file to disk at any time.
