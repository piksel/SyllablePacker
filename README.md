# SyllablePacker

Pack floats in 12-bit chunks

The standard storage type, _Syllable_, uses a single decimal place of precision and has the following constraints:

| Type            | Min value | Max value |
| --------------- | --------- | --------- |
| Unsigned        |    0.0    | 409.4     |
| Signed          | -200.0    | 209.4     | 

Null values are stored as `0xFFF`

## Example

```.cs

// Create a example object
var pre = new ExampleClass()
{
    FloatVal1 = 33.5f,
    FloatVal2 = 200.1f,
    Nullable = null,
    Zero = 0,
    Signed = -75.1f
};

// Initialize the packer
var packer = new SyllablePacker<ExampleClass>()
    .AddProperty(
        o => o.FloatVal1.Syllable(),
        (o, v) => o.FloatVal1 = SyllablePacker.GetFloat(v).Value)
    .AddProperty(
        o => o.FloatVal2.Syllable(),
        (o, v) => o.FloatVal2 = SyllablePacker.GetFloat(v).Value)
    .AddProperty(
        o => o.Nullable.Syllable(),
        (o, v) => o.Nullable = SyllablePacker.GetFloat(v))
    .AddProperty(
        o => o.Zero.Syllable(),
        (o, v) => o.Zero = SyllablePacker.GetFloat(v).Value)
    .AddProperty(
        o => o.Signed.SignedSyllable(),
        (o, v) => o.Signed = SyllablePacker.GetSignedFloat(v).Value);
        
// Pack into bytes
var bytes = packer.Pack(pre);

// Unpack into object
var post = packer.Unpack(bytes);
```
