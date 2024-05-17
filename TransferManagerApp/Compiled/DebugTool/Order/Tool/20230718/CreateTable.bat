@ECHO OFF
SETLOCAL

REM �ڑ����̐ݒ�
SET HOST=localhost
SET PORT=5434
SET DBNAME=transfer_manager_db
SET USER=postgres
SET PGPASSWORD=datalink

REM ���t�擾
SET /P DATE_NAME="�e�[�u�������t���uYYYYMMDD�v�`���œ��͂��ĉ������F"
IF /I {%DATE_NAME%}=={} (GOTO :ERROR)

REM �ϐ�������(����)
SET length=0

REM �����擾�T�u���[�`�������s
CALL :GET_LEN %DATE_NAME%
GOTO :CHECK

REM �����擾
:GET_LEN
  SET str_in=%~1

  :LABEL_TOP
  REM �������Z
  SET /a length+=1
  REM 1�������炷
  SET str_in=%str_in:~1%
  REM �����񂪂Ȃ��Ȃ�����I��
  IF NOT "%str_in%"=="" (GOTO :LABEL_TOP)
EXIT /B

:CHECK
IF /I %length% NEQ 8 (GOTO :ERROR)

REM �e�[�u���쐬�R�}���h
psql -h %HOST% -p %PORT% -U %USER% -d %DBNAME% -f ./CreateTable.sql -v name_date=%DATE_NAME%
GOTO :END

:ERROR
ECHO "���͂�����������܂���"

:END
ENDLOCAL
pause
