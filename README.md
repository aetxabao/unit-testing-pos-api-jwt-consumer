# unit-testing-pos-api-jwt-consumer

Código del cliente que se tiene que conectar con el servidor [PosApiJwt](https://github.com/aetxabao/PosApiJwt "PosApiJwt (GitHub)") utilizando el API del servicio REST con JWT.

## Descripción

El *funcionamiento* del servicio se explica en el siguiente vídeo.

[![IMAGE ALT TEXT](https://img.youtube.com/vi/t0W5lKJcerQ/0.jpg)](https://www.youtube.com/watch?v=t0W5lKJcerQ&list=PLK_BHw0Wm4MKJKynoZf1ph-KpBbzZti_m&index=5 "04. POS Api JWT Consumer")

## Ejercicio

El proyecto contiene dos carpetas PosApiJwtConsumer y PosApiJwtConsumer.Test. 
La primera con el código propio del cliente y la otra con pruebas unitarias.
El código del cliente requiere del paquete RestSharp y las pruebas de una extensión de Xunit.

Para el cliente:

```
cd PosApiJwtConsumer
dotnet add package RestSharp
dotnet run
```

Para las pruebas:

```
cd PosApiJwtConsumer.Test
dotnet add package Xunit.Extensions.Ordering
dotnet test
```

El API del servicio puede ser visible en [Swagger](https://localhost:5001/swagger/index.html "API Swagger") cuando se está ejecutando.

Se tiene que implementar el código de los métodos del fichero [Program.cs](https://github.com/aetxabao/unit-testing-pos-api-jwt-consumer/blob/main/PosApiJwtConsumer/Program.cs "Program.cs (GitHub)") para que el programa funcione correctamente y supere las pruebas.

La solución vendría a ser una convinación de [TodoApiConsumer](https://github.com/aetxabao/TodoApiConsumer "TodoApiConsumer (GitHub)") y 
[OrderApiJwtConsumer](https://github.com/aetxabao/OrderApiJwtConsumer "OrderApiJwtConsumer (GitHub)")

