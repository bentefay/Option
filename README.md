Option
======

What is it?
-----------

A fork of the fantastic [Option](https://github.com/tejacques/Option) library, upgraded to netstandard.

An Option type for C#. It allows you to clearly distinguish between having a value (`Some`) and not having a value (`None`) without using null. 
This can make your APIs much more intuitive by flagging whether clients need to handle an empty result.
It also provides a set of powerful operations to simplify interaction with optional values, which can be more flexible than using the `Try` pattern. 

Unlike [Optional](https://github.com/nlkl/Optional), this library enforces that Option _cannot_ contain `null`, and `null` converts to `Option.None`.
As a result, this library supports unambigious implicit casts from `T`, `null` and `Option.None` to `Option<T>`.

How can I get it?
-----------------

Option is available as a NuGet package: https://www.nuget.org/packages/Option.Netstandard

Example Usage
-------------

### Create an option with a value ###
```csharp
var option = Option.Create(1);
var option = Option.Some(1);
var option = Option<int>.Some(1);
Option<int> option = 1;
```

### Create an empty option ###
```csharp
int? x = null;
var option = Option.Create(x);
var option = Option<int>.None;
Option<int> option = x;
Option<int> option = Option.None;
```

### Check for equality ###
```csharp
Option.Some(1) == Option.Some(1)  // => True
Option.Some(1) == Option.Some(2)  // => False
Option.Some(1) == null  // => False
Option.Some(1) == Option.None  // => False
Option.Empty == null  // => true
```

### Access the option's value imperatively ###
```csharp
Option<int> option = 1;
if (option.TryGetValue(out var value))
    return value.ToString();
else
    return "Empty";
```

```csharp
Option<int> option = 1;
if (option.HasValue)
    return option.Value.ToString();
else
    return "Empty";
```

```csharp
Option<int> option = 1;
var v = option.ValueOrDefault;
var v2 = option.ValueOr(0);
var v3 = option.ValueOr(() => 0); // Deferred factory method (if expensive)
}
```

### Transform the option functionally ###

```csharp
Option<int> option = 1;
return option
    .Match(
        None: () => "Empty",
        Some: value => value.ToString()); // Also supports statements
}
```

``` csharp
Option<int> option = 1;
return option
    .Match()
    .None(() => "Empty")
    .Some(1, () => "One")
    .Some(value => value.ToString())
    .Result(); // Also supports statements
```

``` csharp
Option<int> option = 1;
return option
    .Where(x => x == 1)
    .Select(x => x + 1)
    .SelectMany(x => x == 2 ? Option.Some(x) : Option.None)
    .DefaultIfEmpty(0)
    .Single();
```