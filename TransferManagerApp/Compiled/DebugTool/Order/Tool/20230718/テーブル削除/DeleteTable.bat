@echo off
setlocal enabledelayedexpansion

REM 接続情報の設定
SET HOST=localhost
SET PORT=5434
SET DBNAME=transfer_manager_db
SET USER=postgres
SET PGPASSWORD=datalink

REM 確認メッセージ表示
ECHO データベース内の全テーブルを削除します。よろしいですか? (Y/N)
CHOICE /C YN /M "キーを入力 [Y/N]:"

IF errorlevel 2 (
    ECHO [No]  処理を終了します。
　　PAUSE
    EXIT /b 1
) ELSE (
    ECHO [Yes]  削除処理を行います。。。
)


REM 全テーブル名をForループで取得
for /f "tokens=*" %%a in ('psql -h !HOST! -p !PORT! -d !DBNAME! -U !USER! -w -c "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'" -t -A') do (
  REM 削除するテーブル名を表示
  ECHO %%a
  REM 取得したテーブル名のテーブルを1つ削除
  psql -h %HOST% -p %PORT% -d %DBNAME% -U %USER% -c "DROP TABLE IF EXISTS %%a;"
)
endlocal

ECHO 削除処理が完了しました。
PAUSE