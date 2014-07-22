NSBSubscription
===============

List, aggregate, find and delete NServiceBus subscriptions from Raven

List raw subscriptions
````
NSBSubscription list 
NSBSubscription list where namespace like *sometext*
````

Statistics on subscriptions
````
NSBSubscription -url http://10.0.0.1:8080 statistics -on machine
NSBSubscription -url http://my.raven.host:8081 statistics -on queue
NSBSubscription stat -on typename
````

Delete subscriptions:
````
NSBSubscription delete where machine = 1.2.3.4
NSBSubscription -url http://localhost:8080 delete where queue = someservice and machine = somename
````

Combine it all (note that the criteria is per command):
The back-tick (`) symbol is the PowerShell line-continuation character.
````
NSBSubscription `
    list where queue like my*service `
    stat -on namespace `
    stat -on machine `
    delete where queue = someservice and machine = somename
````
