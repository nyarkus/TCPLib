# What is DataPackageDispatcher and why do I need it?
**DataPackageDispatcher** *(hereafter referred to as DP)* is a very convenient class for listening to packets in certain cases. **DPDispatcher** listens for packets and reacts only to those that pass the filters. When **DPDispatcher** finds a packet that has passed the filter, it calls the methods subscribed to that type of packet.

# Using DPDispatcher
The creation of **DPDispatcher** is roughly the same for both the client and the server, so below is a generalized instruction for using this class.

## DPHandler
To create a DPDispatcher, we first need to create a packet handler.

The packet handler is represented by the class *DPHandler* and is created using the method *Create*:
```csharp
var handler = DPHandler.Create(DPFilter.Equals("Message"), new DataPackageReceive(OnReceived));
```  
*DPFilter* is a filter for the type of packets; in this case, the method will trigger only if the packet type is `Message`.  
*DataPackageReceive* is a delegate for the methods that will be called by DPDispatcher when the filter is triggered.

The method `OnReceived` should have the following structure:
```csharp
Task OnReceived(DataPackageSource package)
```  
*DataPackageSource* is the class of the non-deserialized packet.

## DPDispatcherBuilder

After we have created the packet handlers, we can start constructing **DPDispatcher** using *DPDispatcherBuilder*.

To do this, we need to create a new instance of DPDispatcherBuilder, pass it the __connected__ client/server and the packet handlers:
```csharp
var dispatcher = new DPDispatcherBuilder(client.ConnectedServer, handler)
```  
You can configure your future **DPDispatcher** using methods and various properties of DPDispatcherBuilder. Keep in mind that they cannot be changed later.

Once you have finished configuring, you can create **DPDispatcher** using the `Build` method!

## DPDispatcher
**DPDispatcher** has only 2 methods - `Start` and `Stop`. Use them to control **DPDispatcher**.
