# SyllablePacker

Pack numbers in 12-bit chunks



Value constraints:

| Type              | Min value | Max value |
| ----------------- | --------- | --------- |
| Unsigned Syllable |      0.0  |  409.4    |
| Signed Syllable   |   -200.0  |  209.4    | 
| Unsigned Integer  |      0    | 4094      |
| Signed Integer    |  -2000    | 2094      | 

The standard storage type, _Syllable_, uses a single decimal place of precision.  
Null values are stored as `0xFFF`, non-nullable values should avoid using this value (`4095` decimal).

## Example

```.cs
// Create an example object
var pre = new ExampleClass()
{
    FloatValue = 33.5f,
    DoubleValue = 200.1f,
    DecimalValue = 304.9m,
    Null = null,
    Zero = 0,
    Signed = -75.1f,
    Large = 4094,
    Larger = 16000,
    Time = DateTime.Now
};

// Initialize the packer
var packer = new SyllablePacker<ExampleClass>()
    .AddProperty(o => o.FloatValue.Syllable(),
            (o, v) => o.FloatValue = SyllablePacker.GetFloat(v).Value)
    .AddProperty(o => o.DoubleValue.Syllable(),
            (o, v) => o.DoubleValue = SyllablePacker.GetFloat(v).Value)
    .AddProperty(o => o.Null.Syllable(),                  
            (o, v) => o.Null = SyllablePacker.GetFloat(v))
    .AddProperty(o => o.Zero.Syllable(),                  
            (o, v) => o.Zero = SyllablePacker.GetFloat(v).Value)
    .AddProperty(o => o.Signed.SignedSyllable(),          
            (o, v) => o.Signed = SyllablePacker.GetSignedFloat(v).Value)
    .AddProperty(o => o.DecimalValue.Syllable(),          
            (o, v) => o.DecimalValue = SyllablePacker.GetDecimal(v).Value)
    .AddProperty(o => o.Large.GetValueOrDefault(SyllablePacker.Dash), 
            (o, v) => o.Large = v != SyllablePacker.Dash ? (ushort)v : new ushort?())
    
    // Larger uses two 12-bit values
    .AddProperty(o => o.Larger.GetValueOrDefault(SyllablePacker.Dash),
            (o, v) => o.Larger = v != SyllablePacker.Dash ? v : new long?(), 2)
    
    // Time uses four 12-bit values
    .AddProperty( o => o.Time.Syllables(),
             (o, v) => o.Time = SyllablePacker.GetDateTime(v), 4);
    
// Pack into bytes
var bytes = packer.Pack(pre);

// Unpack into object
var post = packer.Unpack(bytes);
```

The packed bytes would be (being run at 2017-05-26 01:15 UTC+1):  
```14 f7 d1 ff f0 00 4e 1b e9 ff e0 03 e8 00 5c 98 87 31 f8 40 00```

Grouping per three nibbles (12 bit) yields:  
```14f 7d1 fff 000 4e1 be9 ffe 003 e80 05c 988 731 f84 000```  
_Note the last padding nibbles: ``000``_
