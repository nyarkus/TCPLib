# 3.0.0
## What's New?
- [`DPDispatcher`](https://github.com/nyarkus/TCPLib/blob/master/documentation/DataPackageDispatcher.md) for convenient packet handling.
- `DataPackageSource` can now be converted to `DataPackage`.
- `DataPackage` can be converted to `DataPackageSource`.
- Configuration can be specified when creating the server and client.
- Required components can be specified when creating the server.
- Reduction in the number of Receive methods.
- Merging `NetClient` and `Client` into `TCPLib.Server`.
- New variable `IsAlive` in `TCPLib.Server.Net.Client`.
- `IProtobufSerializable` is now `IDataSerializable`.
- New data structure `IP` in `TCPLib.Net` designed to store IP address and port.
- Various minor improvements enhancing usability and code readability.
