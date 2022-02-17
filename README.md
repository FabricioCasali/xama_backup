[![.NET](https://github.com/FabricioCasali/xama_backup/actions/workflows/build.yml/badge.svg)](https://github.com/FabricioCasali/xama_backup/actions/workflows/build.yml)

# xama_backup

Eu criei esta ferramenta para automatizar a cópia do meu ambiente de desenvolvimento de forma rápida e fácil, sem a necessidade de instalar suites complexas (e muitas vezes pagas), e que raramente me davam o poder de filtrar corretamente quais arquivos eu gostaria de excluir ou incluir no backup.

Inicialmente implementei as regras no próprio fonte do programa, porém depois de um tempo eu precisava sempre ficar alterando este código para incluir novos projetos ou novos filtros. 

Assim surgiu a idéia de tornar o programa mais robusto, mais baseado em configurações e menos *hardcoded*. 


## Recursos

* Pode ser instalado como um serviço em ambientes Windows e Linux
* Diversas opções de agendamento (Dia, Mês, Única, Ao iniciar, Semanal, Anual e Cron)
* Cada tarefa permite múltiplos agendamentos
* Três modos de backup: Completo, Diferencial (arquivos alterados desde o último backup completo) e Incremental (arquivos alterados desde o último backup completo ou incremental)
* Regras de retenção de arquivos, eliminando backups antigos
* Compactação do arquivo de saída em ZIP e 7ZIP, podendo escolher o nível de compactação 
* Sistema de regras baseado em padrões para incluir ou excluir arquivos e pastas
