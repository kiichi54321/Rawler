#概要
オレオレフレームワークのオレオレ言語です。XAMLでプログラミングします。
なまえは、Rawlerとしています。
Webからのデータ取得と加工に特化した言語です。いわゆるDSL（ドメイン特化言語）ですね。
ライブラリの取得はNugetから検索でき、簡単に使えます。オープンソースです。
XAMLなので、Visual Studio を使うとIntelliSenseが効果を働き、比較的簡単にできます。
これだけで、Webからのデータ取得からファイルへの書き出しに関してはほとんどできます。
実行環境は、とりあえずWindows専用です。ブラウザからソフトを実行が楽ちんです。
Webからのデータ取得から、形態素解析によるデータ作成といった、テキスト処理が得意です。
通常のサイトなら、慣れると３０分程度でデータ取得のプログラムが組むことができます。
ブラウザゲームのBot作成までも頑張ればできます。

* [全体像の解説PPT(slideshare)](http://www.slideshare.net/TakaichiIto/rawlerv2)
* [git](https://github.com/kiichi54321/Rawler)
* [Nuget](http://www.nuget.org/packages/Rawler/)
* [実行クライアント(ClickOneなのでIEで開くとスムーズ)](http://web.sfc.keio.ac.jp/~kiichi/soft/rawler/publish.htm)
* [基本的な書き方PPT(slideshare)](http://www.slideshare.net/TakaichiIto/rawler-12571428)

#サンプルコード

```xml:sample.xaml
<WebClient Comment="" 
      xmlns="clr-namespace:Rawler.Tool;assembly=Rawler"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      AddUserAgent="True"           
      >
    <!--上はおまじない。下は作業フォルダをMyDocumentsのRawlerというフォルダにする -->
    <SetWorkFolder SpecialFolder="MyDocuments" Folder="Rawler"/>
    <!--最終的にtest.tsvというファイルで保存される-->
    <Data FileName="test.tsv">
        <!--はてなブックマークのトップページの取得-->
        <Page Url="http://b.hatena.ne.jp/">
            <!--このタグで囲まれたところは、取得したHTMLを使って処理をする。-->
            <!--divタグのentry-contents クラスを複数取ってくる-->
            <Tags Tag="div" ClassName="entry-contents">
                <!--タイトル部分の取得-->
                <Tags Tag="h3">
                    <!--余計なタグを消す-->
                    <TagClear ReplaceType="None">
                        <!--上流のDataにtitleという名前で情報を送る-->
                        <DataWrite Attribute="title"></DataWrite>
                    </TagClear>
                    <!--Linkの取得。自動的に絶対パス。本来は複数だが、一つだけなので一回のみ-->
                    <Links>
                        <DataWrite Attribute="url"></DataWrite>
                    </Links>
                </Tags>
                <!--カテゴリの取得-->
                <Tags Tag="li" ClassName="category">
　　　　　　　　　　　 <!--リンクの中身を取得-->
                    <Links VisbleType="Label">
                        <DataWrite Attribute="category"></DataWrite>
                    </Links>                   
                </Tags>
                <!--日付の取得-->
                <Tags Tag="li" ClassName="date">
                    <DataWrite Attribute="date"></DataWrite>
                </Tags>
                <!--上流のDataに次の行に進むことを伝える。イメージとしては改行すること-->
                <NextDataRow/>
            </Tags>
        </Page>
    </Data>
</WebClient>
```
このXMLをコピって、[実行クライアント](http://web.sfc.keio.ac.jp/~kiichi/soft/rawler/publish.htm)にコピペし、「Run」ボタンを押すと実行。最終的には、My DocumentsのRawlerというフォルダに、test.tsvというファイルが作られる。ファイルを読みに行かなくても、ViewDataボタンで内容を見ることができる。

#コンセプト
Webスクレイピングは退屈な作業なので、楽したい。
プログラムのための必須記述を書くのが面倒。newしたり、変数作ったり、foreach文を回したり。
対象サイト用の設定と、プログラムのための記述が混じると、読みにくくなる。
一つのページに対してスクレイピングをするなら、既存のよくあるライブラリで十分だが、
「たくさんのサイトからデータを取得したい」となると、モチベーションが保てない。
IntelliSenseが効いて、メソッドチェーンっぽく何とかならんかと思いながら、作った。（数年前）

#特徴
Webスクレイピングに必要な設定のみを書くだけで、実行可能。
XAMLでテキスト処理の連鎖を書く。XAMLは木構造のため、同じテキストに対する処理を複数書ける。
そのため、関数型言語っぽい、メソッドチェーンができる。
ページを読み込んでリンクを取得して、そのリンクのページを読むというのが、簡単に書ける。
初期設定の簡易化、変数の排除、Foreachの排除。
XAMLなので、Visual StudioでIntelliSenseが効果を発揮する。
設定のみで、コードの見通しはよく、対象サイトの変化に対して対応が楽。ただし、複雑なことをやろうとすると複雑になって大変なことになる。
これのみで、ファイルの読み書き、Webページの取得、解釈といった、Webスクレイピングでやりたいことがすべてできる。
事実上、スクリプト言語なので、プログラム本体と切り離して、記述できるので、スクレイピング機能の外部化ができる。
テキスト処理が得意なので、ついでに簡単な自然言語処理もつけた。


#導入方法
##Visual Studioのインストール
 無償版だと、for windows Desktop版がベストだと思う。
##NugetでRawlerをインストール
Visual Studioを起動し、WPFプロジェクトの作成。そして、Nugetから「Rawler」で検索。インストール。
WPFプロジェクトなのは、一部でWPFを使っているので、最終的に面倒がなさそうなので。
##RawlerTree.xamlを編集
デザインビューは、存在しないので、ぐいっと、XAML画面だけにして、編集する。
##Rawlerの書き方について
[全体像のパワーポイント](http://www.slideshare.net/TakaichiIto/rawlerv2)を読んでください。
基本的な書き方については、[Rawler基本](http://www.slideshare.net/TakaichiIto/rawler-12571428)　を読んでください。

##書いたXAMLの実行
[実行クライアント](http://web.sfc.keio.ac.jp/~kiichi/soft/rawler/publish.htm)に、コピペして実行が一番楽です。
自身のプログラム内で使いたいときは、[RawlerConsole](https://github.com/kiichi54321/Rawler/blob/master/RawlerConsole/Program.cs)を参考にしてください。
一番重要なのは、SetParent()です。上流との通信ができなくなるので、実行ができなくなる。
ちなみに、RawlerConsoleをGitからダウンロードしてコンパイルすると、コマンドラインで実行可能になります。


#コードのサンプル
##ページング
PageとNextPageを組み合わせると、同じページに対しての、再帰的に同じ処理でデータを取得をできる。
終了条件は、「次へ」というリンクがなくなるまで。

```xml:pixiv.xaml
<WebClient Comment="" 
      xmlns="clr-namespace:Rawler.Tool;assembly=Rawler"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      AddUserAgent="True"           
      >
    <!--上はおまじない-->
    <!--作業フォルダをMyDocumentsのRawlerというフォルダにする -->
    <SetWorkFolder SpecialFolder="MyDocuments" Folder="Rawler"/>
    <!--最終的にtest.tsvというファイルで保存される-->
    <Data FileName="test.tsv">
        <Page Url="*******">
            <Tags Tag="li" >
                <DataWrite></DataWrite>
                <NextDataRow/>
            </Tags>
            <!--「次へ」というリンクを取ってくる-->
            <Links LabelFilter="次へ">
                <!--上流のPageに対して、次に読み込むURLを投げて、繰り返す-->
                <NextPage></NextPage>
            </Links>
        </Page>
    </Data>
</WebClient>
```



##認証が必要なサイトにログインする。
POSTもできます。

```xml:pixiv.xaml
<WebClient Comment="" 
      xmlns="clr-namespace:Rawler.Tool;assembly=Rawler"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      AddUserAgent="True"           
      >
    <SetWorkFolder SpecialFolder="MyDocuments" Folder="Rawler"></SetWorkFolder>
    <Data>
        <!--ピクシブのトップ画面からログインフォームを使う-->
        <Page Url="http://www.pixiv.net/" >
            <!--現ページのフォームを読み込みPOSTする-->
            <Page Url="http://www.pixiv.net/login.php" MethodType="POST">
                <!--フォームの入力パラメータを設定する。親のテキストを参照している。この場合、トップページ-->
                <Page.InputParameterTree>                   
                    <!--formタグからlogin.phpが含まれているのを探す-->
                    <Tags Tag="form" ParameterFilter="login.php">
                        <!--フォームに含まれるすべてのHiddenInputParameterを追加-->
                        <AddHiddenInputParameter></AddHiddenInputParameter>
                        <!--ピクシブのID、パスワードを入力-->
                        <AddInputParameter Key="pixiv_id" Value="id"></AddInputParameter>
                        <AddInputParameter Key="pass" Value="パスワード"></AddInputParameter>
                        <AddInputParameter Key="skip" Value="1"></AddInputParameter>
                    </Tags>
                </Page.InputParameterTree>
            </Page>
        </Page>
        <!--ここより下はピクシブへのログイン状態。ログイン情報はWebClientが持っている-->
    </Data>
</WebClient>
```

##ファイルの読み込み
TSVファイルの読み込みはこんなかんじです。
TSVファイルはDataで作られる基本的なデータ形式です。そのため、データを取ってきたものを保存し、その後、読み込むなんていうこともできます。

```xml:pixiv.xaml
<WebClient Comment="" 
      xmlns="clr-namespace:Rawler.Tool;assembly=Rawler"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      AddUserAgent="True"           
      >
    <SetWorkFolder SpecialFolder="MyDocuments" Folder="Rawler"></SetWorkFolder>
    <Data>
        <!--ログイン処理（省略）-->
        
        <!--作業フォルダにある、GetDataList.tsvを読み込む。列は,Label、SearchWord、StartDate、EndDateがある。-->
        <TsvReadLines FileName="GetDataList.tsv">
            <!--ヘッダを除いた、一行づつ読み込まれる。存在する行数分繰り返し-->
            <!--URLを作る。C#のStringFormatと同等-->
            <StringFormat Format="http://www.pixiv.net/search.php?s_mode=s_tag&amp;order=num&amp;word={0}&amp;scd={1}&amp;ecd={2}">
                <!--流し込む文字列を作る-->
                <StringFormat.Args>
                    <RawlerCollection>
                        <!--SearchWord列を読む-->
                        <GetTsvValue ColumnName="SearchWord">
                            <!--URLエンコードをする-->
                            <UrlEncode></UrlEncode>
                        </GetTsvValue>
                        <GetTsvValue ColumnName="StartDate"></GetTsvValue>
                        <GetTsvValue ColumnName="EndDate"></GetTsvValue>
                    </RawlerCollection>
                </StringFormat.Args>
                <!--生成したURLを書き出す-->
                <Report Header="Url:"></Report>
                <!--生成したURLを使ってWebにアクセスする-->
                <Page>
                    <GetTsvValue ColumnName="Label">
                        <DataWrite Attribute="Label"></DataWrite>
                    </GetTsvValue>
                    <!--取得処理(省略)-->
                </Page>
            </StringFormat>
        </TsvReadLines>
    </Data>
</WebClient>
```

##簡単な形態素解析
[http://chasen.org/~taku/software/TinySegmenter/](http://chasen.org/~taku/software/TinySegmenter/)　これのC#版を使い形態素解析をします。

```xml:pixiv.xaml
<WebClient Comment="" 
      xmlns="clr-namespace:Rawler.Tool;assembly=Rawler"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:NPL="clr-namespace:Rawler.NPL;assembly=Rawler"
      AddUserAgent="True"           
      >
    <SetWorkFolder SpecialFolder="MyDocuments" Folder="Rawler"/>
    <Data>
        <Document TextValue="適当なテキスト">
            <!--前処理。半角全角を揃える。大文字小文字を揃えるなど。-->
            <NPL:PreprocessingForJapanese>
                <!--辞書なし軽量形態素解析、TinySegmenterを使う-->
                <NPL:TinySegmenter>
                    <!--標準出力します-->
                    <Report></Report>
                </NPL:TinySegmenter>
            </NPL:PreprocessingForJapanese>
        </Document>
    </Data>
</WebClient>
```



#拡張
たくさんの機能を用意していますが、結局は、自分が取得に困ったときに作っています。
拡張は簡単で、C#でRawlerBaseまたは、RawlerMultiBaseの継承をし、overrideをするだけです。
究極的には、テキストを変数にして、テキストを返せばいいので、何でもありです。
Jsonを返すようにすれば、オブジェクトを渡すことになります。

機能の追加の方法は、

1.それ専用のプロジェクトの追加。
2.NugetでRawlerを取得
3.単数の場合は、RawlerBase,複数の時は、RawlerMultiBaseを継承。
4.Run()をoverrideする。
5.親テキストの取得には、GetText()を使う。
6.XAML編集画面で使うには、コンパイルして、root要素にある、xmlnsに追加してください。

##サンプル
###シンプルな例
タグの中のパラメータを取得する
https://github.com/kiichi54321/Rawler/blob/master/Rawler/Tool/Html/GetTagParameters.cs
単語の指定の区切りで分割する。返しは複数。
https://github.com/kiichi54321/Rawler/blob/master/Rawler/Tool/Text/Split.cs

他のソースコードには、Clone()とObjectNameがテンプレとして存在しますが、結局使いませんでした。あってもなくてもいいです。

###祖先関係で成立する例
XAMLの木構造を使い、祖先にあるオブジェクトに対して効果を発揮するものがいくつかあります。
DataとDataWriteがそうですが、多少ややこしいので、シンプル目のものを紹介します。

Tsvファイルの読み込み。
https://github.com/kiichi54321/Rawler/blob/master/Rawler/Tool/IO/ReadTsv.cs
GetTsvValueクラスのRunメソッドにある、
`var file = this.GetAncestorRawler().OfType<TsvReadLines>();`
で、親のオブジェクトを取得し、そこに問い合わせている。

###Staticクラスにアクセスする例
木構造をたどる場合、必ず、上流にオブジェクトを配置しないといけないのは面倒である。
どこでもほしい時に配置できるのは、やっぱり便利である。
https://github.com/kiichi54321/Rawler/blob/master/Rawler/Tool/Utility/GlobalVar.cs
スコープがなくなり、Staticおじさんとなるので、並列実行したとき、おかしくなると思うが、とりあえず便利です。

#おわりに
そんなわけで使ってみてください。
XAMLはWPFだけのものじゃないよっと。
元々はヴィジュアルプログラミングっぽくやりたかったのですが、XAMLの表現力で満足しました。
あと、よくある質問ですが、Macで動かないのか？というのがあるのですか、特に変な命令を使っていないので、Monoで動くと思うのですが、自分に環境が無いため、よくわかりません。
