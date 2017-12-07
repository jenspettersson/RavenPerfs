RavenDB 4 - 4.0.0-rc-40023 - Community License

settings.json
```
{
  "ServerUrl": "http://localhost:4040",
  "DataDir": "APPDRIVE:/Raven",
  "Indexing": {
    "TimeToWaitBeforeMarkingAutoIndexAsIdleInMin": 1
  },
  "RunInMemory": false
}
```

To reproduce:

Create a database called: `Raven.Index.Debug`

Start the WebAPI (not sure it actually need to be a Web Application, but that's what our application that have this issue is so).

It will run `PopulateData(...)` on startup to populate two document collections (`TestDocument` that uses an IdConvention and `AnotherTestDocument` that doesn't) with 20 documents each.

One auto index for each Document Collection will also be created.

1. Open Raven Studio and look at the index list, both indexes should show "20 entries".
2. Remove a couple of documents manually from each collection, and look at the index list again. It will show the new number of entries correctly.
3. Stop the Web Application (could also remove the `PopulateData` call inside the `Startup` to prevent it from repopulating again) and also, close the Raven Studio (not sure this is needed)
4. Wait... I had to wait between 20-30 minutes for the issue to actually happen.
5. Open Raven Studio and look at the index list again, they will still show "X entries" (depending on how many documents you removed in step 2).
6. Manually remove a couple of documents from each collection again and look at the index list. This time it doesn't always decrement the # of entries. If it does, you probably didn't wait long enough in step 4...
7. Remove ALL documents from the collections and look at the index list again. Here it might differ, but for my latest attempt, the # of entries updated to "7 entries" and "1 entry"

Now, you can start the Web Application again (be sure to remove the call to `PopulateData(...)`). You can do the following requests:

GET: http://localhost:8802/api/anotherTestDocuments

My result:
```
{
    "numberOfActualResults": 0,
    "stats": {
        "isStale": false,
        "durationInMs": 0,
        "totalResults": 7,
        "skippedResults": 7,
        "timestamp": "2017-12-07T11:18:58.3316286",
        "indexName": "Auto/AnotherTestDocuments/ByCreated",
        "indexTimestamp": "2017-12-07T11:18:58.3316286",
        "lastQueryTime": "2017-12-07T11:20:19.8947196",
        "timingsInMs": {},
        "resultEtag": 4441579847025080415,
        "resultSize": 0,
        "scoreExplanations": {}
    }
}
```
The `numberOfActualResults` is the Count of the actual query result, and the stats is what the `.Statistics(...)` is producing.

You can also do a GET: http://localhost:8802/api/testDocuments and get a similar result.

