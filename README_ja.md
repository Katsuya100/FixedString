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
[テストコード](https://github.com/Katsuya100/FixedString/blob/v1.0.0/packages/Tests/Runtime/FixedString16BytesPerformanceTest.cs)  

#### 結果

|  実行処理  |  string  |  FixedString16Bytes  |
| ---- | ---- | ---- |
| CompareTo(完全一致) | 9.19947 ms | 0.13448 ms |
| CompareTo(前方不一致) | 6.774095 ms | 0.31031 ms |
| CompareTo(後方不一致) | 16.82627 ms | 0.29572 ms |
| Equals(完全一致) | 1.9474 ms | 0.170045 ms |
| Equals(前方不一致) | 0.915505 ms | 0.135375 ms |
| Equals(後方不一致) | 1.83779 ms | 0.113755 ms |
| Equals(同一参照) | 0.1114 ms | 0.122265 ms |
| GetHashCode | 0.441135 ms | 0.110915 ms |

殆どの項目で大幅な性能向上が見られています。  

### Releaseビルド後の計測コード
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

#### 結果

|  実行処理  |  string(Mono)  |  FixedString16Bytes(Mono)  |  string(IL2CPP)  |  FixedString16Bytes(IL2CPP)  |
| ---- | ---- | ---- | ---- | ---- |
| CompareTo(完全一致) | 0.2965469 ms | 0.001680374 ms | 0.3044763 ms | 0.0006399155 ms |
| CompareTo(前方不一致) | 0.2367401 ms | 0.006359577 ms | 0.2113404 ms | 0.01101637 ms |
| CompareTo(後方不一致) | 0.5797958 ms | 0.00847435 ms | 0.5714216 ms | 0.01089239 ms |
| Equals(完全一致) | 0.05168152 ms | 0.001331806 ms | 0.02657223 ms | 0 ms(計測不能) |
| Equals(前方不一致) | 0.01606703 ms | 0.001196384 ms | 0.009623051 ms | 0 ms(計測不能) |
| Equals(後方不一致) | 0.05083609 ms | 0.001334667 ms | 0.02079439 ms | 0 ms(計測不能) |
| Equals(同一参照) | 0.001683712 ms | 0.001712799 ms | 0.001599312 ms | 0 ms(計測不能) |
| GetHashCode | 0.01259422 ms | 0.0008416176 ms | 0.01050234 ms | 0 ms(計測不能) |

項目によっては475倍程度性能向上しています。  
EqualsやGetHashCodeに至っては、IL2CPP環境では何度計測しても0msになってしまいます。  

## インストール方法
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

### Burstコンパイル対応
下記のシンボルを定義することにより、FixedStringの実装がBurstコンパイルに対応したコードに置き換わります。  
```
USE_FIXED_STRING_BURST
```
ただし、FixedString実装時点ではSIMD化されると却ってFixedStringの動作が重くなることが確認されております。  
将来的に改善した際にご利用ください。  

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
