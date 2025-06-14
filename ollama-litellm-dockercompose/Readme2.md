Español
ollama-litellm-dockercompose
Este repositorio proporciona archivos de configuración y un archivo docker-compose.yaml para facilitar la ejecución de Open WebUI y LiteLLM con un esfuerzo mínimo.

Contenido del repositorio
.env: Archivo de variables de entorno donde deben añadirse las claves API de los proveedores de modelos (como OpenAI, Anthropic, OpenRouter, etc.).
docker-compose.yaml: Archivo de configuración de Docker Compose.
litellm_config.yaml: Archivo de configuración para LiteLLM, donde se pueden añadir y configurar modelos adicionales según las necesidades.
Requisitos previos
Docker
Instrucciones de uso
Clonar el repositorio:

git clone https://github.com/gmag11/ollama-litellm-dockercompose.git
cd ollama-litellm-dockercompose
Configurar las variables de entorno:

Edite el archivo .env para añadir las claves API de los proveedores de modelos necesarios (como OpenAI, Anthropic, OpenRouter, entre otros) según los modelos que planee utilizar.

Configurar modelos adicionales (opcional):

Puede añadir más modelos en el archivo litellm_config.yaml, siguiendo las instrucciones de configuración disponibles en la documentación de LiteLLM. Esto permite personalizar y ampliar los modelos que estarán disponibles en LiteLLM.

Iniciar los servicios:

docker-compose up -d
Esto iniciará los servicios en segundo plano.

Acceder a Open WebUI:

Abra su navegador web y navegue a http://localhost:3000 para acceder a Open WebUI.

Notas adicionales
Asegúrese de que los puertos especificados en el archivo docker-compose.yaml no estén en uso por otros servicios en su sistema.

Para detener los servicios, ejecute:

docker-compose down
English
ollama-litellm-dockercompose
This repository provides configuration files and a docker-compose.yaml file to facilitate running Open WebUI and LiteLLM with minimal effort.

Repository Contents
.env: Environment variables file where API keys for model providers (such as OpenAI, Anthropic, OpenRouter, etc.) should be added.
docker-compose.yaml: Docker Compose configuration file.
litellm_config.yaml: Configuration file for LiteLLM, where additional models can be added and configured as needed.
Prerequisites
Docker
Usage Instructions
Clone the repository:

git clone https://github.com/gmag11/ollama-litellm-dockercompose.git
cd ollama-litellm-dockercompose
Configure environment variables:

Edit the .env file to add the necessary API keys for the model providers you plan to use (such as OpenAI, Anthropic, OpenRouter, among others).

Configure additional models (optional):

You can add more models in the litellm_config.yaml file, following the configuration instructions available in LiteLLM documentation. This allows customization and expansion of models available in LiteLLM.

Start the services:

docker-compose up -d
This will start the services in the background.

Access Open WebUI:

Open your web browser and navigate to http://localhost:8080 to access Open WebUI.

Additional Notes
Ensure that the ports specified in the docker-compose.yaml file are not in use by other services on your system.

To stop the services, run:

docker-compose down