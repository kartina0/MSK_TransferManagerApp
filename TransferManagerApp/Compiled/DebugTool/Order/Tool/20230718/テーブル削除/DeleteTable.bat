@echo off
setlocal enabledelayedexpansion

REM �ڑ����̐ݒ�
SET HOST=localhost
SET PORT=5434
SET DBNAME=transfer_manager_db
SET USER=postgres
SET PGPASSWORD=datalink

REM �m�F���b�Z�[�W�\��
ECHO �f�[�^�x�[�X���̑S�e�[�u�����폜���܂��B��낵���ł���? (Y/N)
CHOICE /C YN /M "�L�[����� [Y/N]:"

IF errorlevel 2 (
    ECHO [No]  �������I�����܂��B
�@�@PAUSE
    EXIT /b 1
) ELSE (
    ECHO [Yes]  �폜�������s���܂��B�B�B
)


REM �S�e�[�u������For���[�v�Ŏ擾
for /f "tokens=*" %%a in ('psql -h !HOST! -p !PORT! -d !DBNAME! -U !USER! -w -c "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'" -t -A') do (
  REM �폜����e�[�u������\��
  ECHO %%a
  REM �擾�����e�[�u�����̃e�[�u����1�폜
  psql -h %HOST% -p %PORT% -d %DBNAME% -U %USER% -c "DROP TABLE IF EXISTS %%a;"
)
endlocal

ECHO �폜�������������܂����B
PAUSE