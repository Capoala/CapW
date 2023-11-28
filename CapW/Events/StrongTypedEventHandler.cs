namespace CapW.Events;

public delegate void StrongTypedEventHandler<TSender, TResult>(TSender sender, TResult args);
