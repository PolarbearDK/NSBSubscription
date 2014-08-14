NSBSubscription
===============

Command line tool to List, aggregate, find and delete NServiceBus subscriptions from Raven

List all options (help)
````
NSBSubscription -? 
or
NSBSubscription -help
````

List raw subscriptions

```posh
NSBSubscription list 
NSBSubscription list where namespace like *sometext*
````

Statistics on subscriptions

```posh
NSBSubscription -url http://10.0.0.1:8080 statistics -on machine
NSBSubscription -url http://my.raven.host:8081 statistics -on queue
NSBSubscription stat -on typename
```

Delete subscriptions (Seems to be working, but check result!):

```posh
NSBSubscription delete where machine = 1.2.3.4
NSBSubscription -url http://localhost:8080 delete where queue = someservice and machine = somename
```

Combine it all (note that the criteria is per command):
The back-tick (`) symbol is the PowerShell line-continuation character.

```posh
NSBSubscription `
    list where queue like my*service `
    stat -on namespace `
    stat -on machine `
    delete where queue = someservice and machine = somename
```
