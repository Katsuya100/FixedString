# FixedString
## 概要
本ライブラリ「FixedString」は高速な固定長文字列を提供します。  
文字列を固定長化することで比較処理やハッシュ化を高速化しており  
要所に組み込むことでゲームのパフォーマンス向上が期待できます。  

## 動作確認環境
|  環境  |  バージョン  |
| ---- | ---- |
| Unity | 2021.3.15f1, 2022.3.2f1 |
| .Net | 4.x, Standard 2.1 |

## 性能
### エディタ上の計測コード
[テストコード](packages/Tests/Runtime/FixedString16BytesPerformanceTest.cs)  

#### 結果

|  実行処理  |  string  |  FixedString16Bytes  |
| ---- | ---- | ---- |
| CompareTo(完全一致) | 8.93387 ms | 0.14452 ms |
| CompareTo(前方不一致) | 6.730895 ms | 0.18746 ms |
| CompareTo(後方不一致) | 1.99403 ms | 0.2018 ms |
| Equals(完全一致) | 1.9474 ms | 0.13075 ms |
| Equals(前方不一致) | 0.922145 ms | 0.135375 ms |
| Equals(後方不一致) | 1.932745 ms | 0.11946 ms |
| Equals(同一参照) | 0.117455 ms | 0.12779 ms |
| GetHashCode | 0.434235 ms | 0.1588 ms |

殆どの項目で大幅な性能向上が見られています。  

### Releaseビルド後の計測コード
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

#### 結果

|  実行処理  |  string(Mono)  |  FixedString16Bytes(Mono)  |  string(IL2CPP)  |  FixedString16Bytes(IL2CPP)  |
| ---- | ---- | ---- | ---- | ---- |
| CompareTo(完全一致) | 2.556774 ms | 0.01568604 ms | 3.039063 ms | 0.006835938 ms |
| CompareTo(前方不一致) | 2.155987 ms | 0.03159332 ms | 2.127686 ms | 0.01342773 ms |
| CompareTo(後方不一致) | 5.269791 ms | 0.03646469 ms | 6.133057 ms | 0.01928711 ms |
| Equals(完全一致) | 0.4704475 ms | 0.01257324 ms | 0.2084961 ms | 0 ms(計測不能) |
| Equals(前方不一致) | 0.1491051 ms | 0.01111221 ms | 0.09838867 ms | 0 ms(計測不能) |
| Equals(後方不一致) | 0.4700203 ms | 0.01218033 ms | 0.2211914ms | 0 ms(計測不能) |
| Equals(同一参照) | 0.01568604 ms | 0.01568222 ms | 0.01318359 ms | 0 ms(計測不能) |
| GetHashCode | 0.1163826 ms | 0.02494431 ms | 0.05493164 ms | 0.0002441406 ms |

項目によっては440倍程度性能向上しています。  
Equalsに至っては、IL2CPP環境では何度計測しても0msになってしまいます。  

## インストール方法
### 依存パッケージをインストール
以下のパッケージをインストールする。  

- [ILPostProcessorCommon v1.2.0](https://github.com/Katsuya100/ILPostProcessorCommon/tree/v1.2.0)
- [ConstExpressionForUnity v2.1.1](https://github.com/Katsuya100/ConstExpressionForUnity/tree/v2.1.1)

### FixedStringのインストール
1. [Window > Package Manager]を開く。
2. [+ > Add package from git url...]をクリックする。
3. `https://github.com/Katsuya100/FixedString.git?path=packages`と入力し[Add]をクリックする。

#### うまくいかない場合
上記方法は、gitがインストールされていない環境ではうまく動作しない場合があります。
[Releases](https://github.com/Katsuya100/FixedString/releases)から該当のバージョンの`com.katuusagi.fixedstring.tgz`をダウンロードし
[Package Manager > + > Add package from tarball...]を使ってインストールしてください。

#### それでもうまくいかない場合
[Releases](https://github.com/Katsuya100/FixedString/releases)から該当のバージョンの`Katuusagi.FixedString.unitypackage`をダウンロードし
[Assets > Import Package > Custom Package]からプロジェクトにインポートしてください。

## 使い方
### 通常の使用法
以下の記法でFixedStringを使用できます。  
```.cs
Katuusagi.FixedString.FixedString16Bytes str = "hoge";
```

### SerializeFieldにする
クラスメンバのSerializeFieldにすることも可能です。  
```.cs
[SerializeField]
private Katuusagi.FixedString.FixedString16Bytes _str = "hoge";
```

### マルチエンディアン対応
FixedStringのGetHashCodeは異なるエンディアン間で異なる値を返却します。  
通信などでハッシュ値を使用する場合は下記のシンボルを定義してください。  
```
FIXED_STRING_ENDIAN_SAFE
```
シンボルを定義することによりビッグエンディアン環境でエンディアン変換が行われるようになります。  
ただし、GetHashCodeの処理速度がやや下がります。  

## 高速な理由
stringはchar型のコレクションとなっており、比較処理には文字数に応じたループ処理が行われています。  
コンパイラによる最適化で実行時速度はかなり向上していますが、GetHashCode等複雑な処理が挟まる際には馬鹿にできないコストが嵩みます。  

FixedStringでは文字列をUTF-8のbyte配列として解釈し、16byteのサイズに限定することで高速化を実現しています。  
long型2つをメモリに割り当てているため少コストでの比較が可能です。  

また、ド・ブラウン列を使った高速ビットスキャンによってO(2)のCompareToを実現しました。  

比較以外の処理(型変換や文字列長の取得)については通常よりオーバーヘッドが発生する部分もあります。  
ただ、こちらも高速ビットスキャンやstackalloc等のテクニックを用いてオーバーヘッドを削減しています。  

`MethodImpl`属性で`AggressiveInline`を設定しているため、ビルド時のインライン展開による最適化も期待できます。  

以上のテクニックにより従来の文字列比較処理に比べ圧倒的なパフォーマンスを実現しています。  
