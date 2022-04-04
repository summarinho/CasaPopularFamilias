# CasaPopularFamilias

## Objetivo
   - Calcular uma quantidade de pontos através da renda e do numero de dependentes
   - Demonstrar habilidades com dotnet core (aqui usei versão 5)
    
## Requisitos / Dependências:
  - Ter conexão com a internet para acesso a API no caso de uma (nesse caso não é necessario pois a api é fake)
  
## Ambientes
  - Swagger (apenas em DESENVOLVIMENTO)
     - /swagger
## Arquitetura
  - Clean Architecture
  
## Considerações
	- Repeti um método no desenvolvimento do service somente para ter um método retornando uma lista e um retornando um objeto
	- Coloquei um header nas chamadas do Refit só pra exemplificar o uso, por isso deixei as informações hardcode lá
	- Deixei o código o mais simples possivel pois eu prefiro códigos simples e objetivos
	- As regras de negócio poderiam ser um pouco mais claras, por exemplo, uma familia sem renda, mas com dependentes pode pontuar? Entendi que sim, já que no enunciado 
	fala sobre ganhar a casa, porém regras mais claras geram menos problemas de entendimento, consequentemente, menos retrabalho.
  

   