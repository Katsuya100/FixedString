# FixedString
[日本語](README_ja.md)

## Summary
This library "FixedString" provides fast fixed-length strings.  
By making strings fixed-length, it speeds up comparison processing and hashing.  
By incorporating this library into your games, you can expect to improve the performance of your games.  

## System Requirements
|  Environment  |  Version  |
| ---- | ---- |
| Unity | 2021.3.15f1, 2022.3.2f1 |
| .Net | 4.x, Standard 2.1 |

## Performance
### Measurement code on the editor
[Test Code.](packages/Tests/Runtime/FixedString16BytesPerformanceTest.cs)  

#### Result

|  Process  |  string  |  FixedString16Bytes  |
| ---- | ---- | ---- |
| CompareTo(ExactMatch) | 8.93387 ms | 0.14452 ms |
| CompareTo(FirstMismatch) | 6.730895 ms | 0.18746 ms |
| CompareTo(LastMismatch) | 1.99403 ms | 0.2018 ms |
| Equals(ExactMatch) | 1.9474 ms | 0.13075 ms |
| Equals(FirstMismatch) | 0.922145 ms | 0.135375 ms |
| Equals(LastMismatch) | 1.932745 ms | 0.11946 ms |
| Equals(SameReference) | 0.117455 ms | 0.12779 ms |
| GetHashCode | 0.434235 ms | 0.1588 ms |

Significant performance improvements have been seen in most items.  

### Measurement code on the runtime
```.cs
private readonly ref struct Measure
{
    private readonly string _label;
    private readonly StringBuilder _builder;
    private readonly float _time;

    public Measure(string label, StringBuilder builder)
    {
        _label = label;
        _builder = builder;
        _time = (Time.realtimeSinceStartup * 1000);
    }

    public void Dispose()
    {
        _builder.AppendLine($"{_label}: {(Time.realtimeSinceStartup * 1000) - _time} ms");
    }
}

private const string Str16              = "1234567890123456";
private const string StrFirstMismatch   = "0234567890123456";
private const string StrLastMismatch    = "1234567890123450";

:

var log = new StringBuilder();
int testCount = 10000;

string str16 = Str16;
Katuusagi.FixedString.FixedString16Bytes fstr16 = Str16;

{
    string cmp = new string(Str16);
    using (new Measure("string.CompareTo(ExactMatch)", log))
    {
        for (int i = 0; i < testCount; ++i)
        {
            str16.CompareTo(cmp);
        }
    }

    Katuusagi.FixedString.FixedString16Bytes fcmp = Str16;
    using (new Measure("FixedString16Bytes.CompareTo(ExactMatch)", log))
    {
        for (int i = 0; i < testCount; ++i)
        {
            fstr16.CompareTo(fcmp);
        }
    }
}
 :
```

#### Result
|  Process  |  string(Mono)  |  FixedString16Bytes(Mono)  |  string(IL2CPP)  |  FixedString16Bytes(IL2CPP)  |
| ---- | ---- | ---- | ---- | ---- |
| CompareTo(ExactMatch) | 2.556774 ms | 0.01568604 ms | 3.039063 ms | 0.006835938 ms |
| CompareTo(FirstMismatch) | 2.155987 ms | 0.03159332 ms | 2.127686 ms | 0.01342773 ms |
| CompareTo(LastMismatch) | 5.269791 ms | 0.03646469 ms | 6.133057 ms | 0.01928711 ms |
| Equals(ExactMatch) | 0.4704475 ms | 0.01257324 ms | 0.2084961 ms | 0 ms(unmeasurable) |
| Equals(FirstMismatch) | 0.1491051 ms | 0.01111221 ms | 0.09838867 ms | 0 ms(unmeasurable) |
| Equals(LastMismatch) | 0.4700203 ms | 0.01218033 ms | 0.2211914ms | 0 ms(unmeasurable) |
| Equals(SameReference) | 0.01568604 ms | 0.01568222 ms | 0.01318359 ms | 0 ms(unmeasurable) |
| GetHashCode | 0.1163826 ms | 0.02494431 ms | 0.05493164 ms | 0.0002441406 ms |

Some items improve performance by about 440x.  
When it comes to Equals, it is 0 ms no matter how many times it is measured in the IL2CPP environment.  

## How to install
### Install dependenies
Install the following packages.  

- [ILPostProcessorCommon v1.1.0](https://github.com/Katsuya100/ILPostProcessorCommon/tree/v1.1.0)
- [ConstExpressionForUnity v2.1.0](https://github.com/Katsuya100/ConstExpressionForUnity/tree/v2.1.0)

### Installing FixedString
1. Open [Window > Package Manager].
2. click [+ > Add package from git url...].
3. Type `https://github.com/Katsuya100/FixedString.git?path=packages` and click [Add].

#### If it doesn't work
The above method may not work well in environments where git is not installed.  
Download the appropriate version of `com.katuusagi.fixedstring.tgz` from [Releases](https://github.com/Katsuya100/FixedString/releases), and then [Package Manager > + > Add package from tarball...] Use [Package Manager > + > Add package from tarball...] to install the package.

#### If it still doesn't work
Download the appropriate version of `Katuusagi.FixedString.unitypackage` from [Releases](https://github.com/Katsuya100/FixedString/releases) and Import it into your project from [Assets > Import Package > Custom Package].

## How to Use
### Normal usage
It can use FixedString with the following notation.  
```.cs
Katuusagi.FixedString.FixedString16Bytes str = "hoge";
```

### Make it a SerializeField
It can also be a class member SerializeField.  
```.cs
[SerializeField]
private Katuusagi.FixedString.FixedString16Bytes _str = "hoge";
```

### Multi-endian support
FixedString GetHashCode returns different values across different endians.  
To use hash values for communication and other purposes, define the following symbols.  
```
FIXED_STRING_ENDIAN_SAFE
```
By defining a symbol, endian conversion is performed in a big-endian environment.  
However, the processing speed of GetHashCode is slightly reduced.  

### Burst compilation support
By defining the following symbols, the FixedString implementation replaces the code for Burst compilation.  
```
USE_FIXED_STRING_BURST
```
However, at the time of FixedString implementation, it has been confirmed that the operation of FixedString will become heavier if it is converted to SIMD.  
Please use it when it is improved in the future.  

## Reasons for high performance
The string is a collection of char types, and the comparison process is looped according to the number of characters.  
Although compiler optimization has improved the runtime speed considerably, the cost is not insignificant when complex processing such as GetHashCode is intervened.  

FixedString interprets strings as UTF-8 byte arrays and limits the size to 16 bytes to achieve high speed.  
Two long types are allocated in memory, allowing comparison at a small cost.  

In addition, O(2) CompareTo is achieved by high-speed bit-scanning using de Bruijn sequence.  

Some processes other than comparison (type conversion and string length acquisition) incur more overhead than usual.  
However, this overhead is also reduced by using techniques such as fast bit scan and stackalloc.  

Since `AggressiveInline` is set in the `MethodImpl` attribute, optimization can be expected from inline expansion at build time.  

The above techniques provide unparalleled performance compared to conventional string comparison processing.  
