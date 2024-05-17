@ECHO OFF
SETLOCAL

REM 接続情報の設定
SET HOST=localhost
SET PORT=5434
SET DBNAME=transfer_manager_db
SET USER=postgres
SET PGPASSWORD=datalink

REM 日付取得
SET /P DATE_NAME="テーブル名日付を「YYYYMMDD」形式で入力して下さい："
IF /I {%DATE_NAME%}=={} (GOTO :ERROR)

REM 変数初期化(長さ)
SET length=0

REM 長さ取得サブルーチンを実行
CALL :GET_LEN %DATE_NAME%
GOTO :CHECK

REM 長さ取得
:GET_LEN
  SET str_in=%~1

  :LABEL_TOP
  REM 長さ加算
  SET /a length+=1
  REM 1文字減らす
  SET str_in=%str_in:~1%
  REM 文字列がなくなったら終了
  IF NOT "%str_in%"=="" (GOTO :LABEL_TOP)
EXIT /B

:CHECK
IF /I %length% NEQ 8 (GOTO :ERROR)

REM テーブル作成コマンド
psql -h %HOST% -p %PORT% -U %USER% -d %DBNAME% -f ./CreateTable.sql -v name_date=%DATE_NAME%
GOTO :END

:ERROR
ECHO "入力が正しくありません"

:END
ENDLOCAL
pause
