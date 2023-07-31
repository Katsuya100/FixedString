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
[Test Code.](https://github.com/Katsuya100/FixedString/packages/Tests/Runtime/FixedString16BytesPerformanceTest.cs)  

#### Result

|  Process  |  string  |  FixedString16Bytes  |
| ---- | ---- | ---- |
| CompareTo(ExactMatch) | 9.19947 ms | 0.13448 ms |
| CompareTo(FirstMismatch) | 6.774095 ms | 0.31031 ms |
| CompareTo(LastMismatch) | 16.82627 ms | 0.29572 ms |
| Equals(ExactMatch) | 1.9474 ms | 0.170045 ms |
| Equals(FirstMismatch) | 0.915505 ms | 0.135375 ms |
| Equals(LastMismatch) | 1.83779 ms | 0.113755 ms |
| Equals(SameReference) | 0.1114 ms | 0.122265 ms |
| GetHashCode | 0.441135 ms | 0.110915 ms |

Significant performance improvements have been seen in most items.  

### Measurement code on the runtime
```.cs
float time;
var log = new StringBuilder();
int testCount = 1000000;
string s = "1234567890123456";
Katuusagi.FixedString.FixedString16Bytes fs = "1234567890123456";
{
    time = Time.realtimeSinceStartup;
    for (int i = 0; i < testCount; ++i)
    {
        s.GetHashCode();
    }
    log.AppendLine($"string.GetHashCode: {Time.realtimeSinceStartup - time} ms");

    time = Time.realtimeSinceStartup;
    for (int i = 0; i < testCount; ++i)
    {
        fs.GetHashCode();
    }
    log.AppendLine($"FixedString16Bytes.GetHashCode: {Time.realtimeSinceStartup - time} ms");
}
 :
```

#### Result
|  Process  |  string(Mono)  |  FixedString16Bytes(Mono)  |  string(IL2CPP)  |  FixedString16Bytes(IL2CPP)  |
| ---- | ---- | ---- | ---- | ---- |
| CompareTo(ExactMatch) | 0.2965469 ms | 0.001680374 ms | 0.3044763 ms | 0.0006399155 ms |
| CompareTo(FirstMismatch) | 0.2367401 ms | 0.006359577 ms | 0.2113404 ms | 0.01101637 ms |
| CompareTo(LastMismatch) | 0.5797958 ms | 0.00847435 ms | 0.5714216 ms | 0.01089239 ms |
| Equals(ExactMatch) | 0.05168152 ms | 0.001331806 ms | 0.02657223 ms | 0 ms(unmeasurable) |
| Equals(FirstMismatch) | 0.01606703 ms | 0.001196384 ms | 0.009623051 ms | 0 ms(unmeasurable) |
| Equals(LastMismatch) | 0.05083609 ms | 0.001334667 ms | 0.02079439 ms | 0 ms(unmeasurable) |
| Equals(SameReference) | 0.001683712 ms | 0.001712799 ms | 0.001599312 ms | 0 ms(unmeasurable) |
| GetHashCode | 0.01259422 ms | 0.0008416176 ms | 0.01050234 ms | 0 ms(unmeasurable) |

Some items have improved performance by a factor of about 475.  
For Equals and GetHashCode, the IL2CPP environment gives 0 ms no matter how many times we measure it.  

## How to install
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
