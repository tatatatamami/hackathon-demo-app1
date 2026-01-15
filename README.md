# hackathon-demo-app1

ハッカソンデモ用チャットアプリケーション

## 概要

このアプリケーションは、.NET 8 Blazor Web Appで構築されたデモ用チャットUIです。
複数のゲームエージェント（Game A～E）を選択できる機能を提供します。

**Game A Agent** は Copilot Studio の Webchat（iframe埋め込み）を表示し、実際のエージェントと会話できます。
Game B～E Agent は「Coming Soon」として表示されます。

## 技術スタック

- .NET 8
- Blazor Web App (Server)
- 標準ライブラリのみ（依存関係なし）
- カスタムCSS

## 機能

### UI構成

- **左サイドバー（300px）**
  - エージェント一覧（Game A Agent ～ Game E Agent）
  - 選択中のエージェントは紫色でハイライト
  - 下部にユーザー名「Tamami」を表示

- **右メインエリア**
  - ヘッダー：選択中のエージェント名と「Online」ステータス
  - **Game A Agent選択時**：
    - Copilot Studio Webchat を iframe で表示
    - ローディングインジケーター表示（読み込み中）
    - iframe は角丸デザインで全画面表示
  - **Game B～E Agent選択時**：
    - 「権限なし」メッセージを表示
    - 入力欄は無効化

### Copilot Studio Webchat 設定

Game A Agent で表示する Copilot Studio の Webchat URL は設定ファイルで指定します。

#### 設定方法1: appsettings.json を編集

`appsettings.json` に以下のセクションを追加または編集：

```json
{
  "CopilotStudio": {
    "GameAWebChatUrl": "https://copilotstudio.microsoft.com/environments/YOUR_ENV/bots/YOUR_BOT/webchat?__version__=2"
  }
}
```

#### 設定方法2: 環境変数を使用

環境変数で上書きすることも可能です：

```bash
# Linux/Mac
export CopilotStudio__GameAWebChatUrl="https://copilotstudio.microsoft.com/environments/YOUR_ENV/bots/YOUR_BOT/webchat?__version__=2"

# Windows (PowerShell)
$env:CopilotStudio__GameAWebChatUrl="https://copilotstudio.microsoft.com/environments/YOUR_ENV/bots/YOUR_BOT/webchat?__version__=2"
```

**注意**: 環境変数名では `:` の代わりに `__`（ダブルアンダースコア）を使用します。

### シナリオ機能

シナリオはJSON形式で管理され、差し替え可能です。

**ファイル配置：** `wwwroot/scripts/scenarios.json`

### 返信判定ルール（優先順）

1. **画像が添付されている場合** → 課題1（kadai1）として処理
2. **テキストに「瞬間移動」が含まれる** → 課題2（kadai2）として処理
3. **テキストに「全選択」「記録」「フラグ」が含まれる** → 課題3（kadai3）として処理
4. **その他** → デフォルトガイドメッセージを返す

### 課題内の照合ルール

- まず `userExact` で完全一致を確認
- 一致しなければ `keywords` で部分一致（いずれかのキーワードを含む）
- それでも一致しなければ、その課題の最初のTurnを返す（デモが壊れないように）

### デモ演出

- 返信時に500〜900msのランダム遅延
- "typing…" インジケーター表示

## 起動方法

### 前提条件

- .NET 8 SDK がインストールされていること

### 手順

1. リポジトリをクローン

```bash
git clone https://github.com/tatatatamami/hackathon-demo-app1.git
cd hackathon-demo-app1
```

2. アプリケーションをビルドして実行

```bash
dotnet restore
dotnet run
```

3. ブラウザで `https://localhost:5001` または `http://localhost:5000` にアクセス

### 開発モードでの起動

```bash
dotnet watch run
```

ファイル変更時に自動的に再ビルド・再起動されます。

## シナリオの差し替え方法

1. `wwwroot/scripts/scenarios.json` ファイルを編集
2. アプリケーションを再起動

### scenarios.json の構造

```json
{
  "scenarios": [
    {
      "agentName": "Game A Agent",
      "kadai": [
        {
          "kadaiId": "kadai1",
          "turns": [
            {
              "userExact": "完全一致する質問文",
              "keywords": ["キーワード1", "キーワード2"],
              "agentReply": "エージェントの返信"
            }
          ]
        }
      ]
    }
  ]
}
```

### フィールドの説明

- `agentName`: エージェント名（"Game A Agent" など）
- `kadaiId`: 課題ID（"kadai1", "kadai2", "kadai3"）
- `userExact`: ユーザー入力と完全一致する文字列（任意、nullも可）
- `keywords`: 部分一致で検索するキーワード配列（任意、nullも可）
- `agentReply`: エージェントの返信メッセージ

## プロジェクト構成

```
/
├── Components/
│   ├── Pages/
│   │   └── Home.razor          # メインのチャットUI
│   ├── Layout/                 # レイアウトコンポーネント
│   ├── App.razor               # ルートアプリケーションコンポーネント
│   └── _Imports.razor          # 共通のusing宣言
├── Models/
│   ├── ChatMessage.cs          # チャットメッセージモデル
│   ├── CopilotStudioOptions.cs # Copilot Studio設定クラス
│   └── ScenarioModels.cs       # シナリオデータモデル
├── Services/
│   ├── ScenarioRepository.cs   # シナリオJSONの読み込み
│   └── ReplyEngine.cs          # 返信判定ロジック
├── wwwroot/
│   ├── css/
│   │   └── site.css            # カスタムCSS
│   └── scripts/
│       └── scenarios.json      # シナリオデータ
├── Program.cs                  # アプリケーションエントリーポイント
└── ChatApp.csproj              # プロジェクトファイル
```

## 注意事項

- **Game A Agent** は Copilot Studio Webchat（iframe）を表示します
  - 正しい Webchat URL を設定する必要があります
  - iframe がブロックされる場合は、別の埋め込み方式を検討してください
- **Game B～E Agent** は「Coming Soon」として表示され、アクセスが制限されています
- スタブ応答ロジック（ReplyEngine/ScenarioRepository）は将来の拡張用に保持されていますが、現在は使用されていません

## ライセンス

MIT License
