# DPlus projeto exclusivo - ClientsApp

![Ícone do Aplicativo](./docs/app_icon_readme.svg)

Este é um projeto de aplicativo para gerenciamento de clientes, desenvolvido com .NET MAUI e estilizado com um tema inspirado em Cyberpunk 2077.

![Lista de clientes](./App%20Images/Screenshot_lista%20de%20clientes.png)

![Novo cliente](./App%20Images/Screenshot_novo%20cliente.png)

![Editar cliente](./App%20Images/Screenshot_editar%20cliente.png)

![Excluir cliente](./App%20Images/Screenshot_excluir%20cliente.png)

==================================================
DOCUMENTAÇÃO DA PAGINA: SIMULADOR ESTOCÁSTICO
==================================================

## Visão Geral do Projeto

Este é um aplicativo .NET MAUI projetado para visualizar simulações de processos estocásticos, especificamente o Movimento Browniano Geométrico. Ele permite que o usuário defina parâmetros financeiros (preço inicial, volatilidade, drift), personalize a aparência do gráfico e execute simulações para ver as possíveis trajetórias de preço ao longo do tempo.

## Arquitetura

O projeto segue o padrão de arquitetura MVVM (Model-View-ViewModel), que promove uma clara separação de responsabilidades:

- **Models:** Contêm os dados brutos e a lógica de negócio. São classes simples que representam as entidades do aplicativo.
- **Views:** A interface do usuário (UI). São responsáveis apenas pela apresentação dos dados e por capturar a interação do usuário.
- **ViewModels:** Atuam como uma ponte entre a View e o Model. Contêm a lógica de apresentação e o estado da UI, expondo dados e comandos para a View.
- **Services:** Fornecem funcionalidades específicas e desacopladas, como cálculos complexos ou acesso a APIs do dispositivo.

## Componentes Principais

### 1. Models

- **BrownianDataModel.cs:** Uma classe simples que agrupa todos os parâmetros de entrada necessários para uma simulação (Preço Inicial, Sigma, Mean, NumDays).
- **ColorOptionModel.cs:** Representa uma opção de cor na interface, com um nome amigável (ex: "Cyber Cyan") e seu valor hexadecimal.

### 2. Views

- **BrownianMotionPage.xaml:** A única tela do aplicativo.
  - **Layout:** Organizada em um Grid de duas colunas: o painel de controle à esquerda e a área do gráfico à direita.
  - **Controles:** Utiliza Entries para entrada de dados, Sliders para controle de período e número de simulações, um Picker para seleção de cor e Switches para opções visuais.
  - **Gráfico:** Usa o componente `ChartView` da biblioteca `Microcharts.Maui` para renderizar a simulação.

### 3. ViewModels

- **BrownianMotionViewModel.cs:** O cérebro da aplicação.
  - **Propriedades:** Expõe todas as propriedades que a View precisa para se vincular (bind), como `InitialPrice`, `Sigma`, `NumberOfSimulations`, `SelectedColor`, e o próprio `SimulationChart`.
  - **Comandos:**
    - `StartSimulationCommand`: Acionado pelo botão "[ Rodar Simulação ]". Orquestra todo o processo de validação, cálculo e exibição.
    - `ClearSimulationCommand`: Limpa o gráfico da tela.
  - **Lógica de Múltiplas Simulações (Ponto de Atenção):** A lógica atual com `step` e `Task.Delay` tenta executar múltiplas simulações de forma sequencial com um atraso. No entanto, como cada chamada redesenha o mesmo `SimulationChart`, o resultado é que cada novo gráfico sobrescreve o anterior, e o usuário vê apenas o último. Para exibir múltiplos gráficos, seria necessário armazená-los em uma coleção (`ObservableCollection<Chart>`) e exibi-los em um `BindableLayout`.

### 4. Services

- **IBrownianService / BrownianService.cs:** Responsável exclusivamente pelo cálculo matemático da simulação. Recebe os parâmetros através do `BrownianDataModel` e um gerador `Random`, e retorna um array de `double` com os valores da trajetória.
- **IDialogService / DialogService.cs:** Abstrai a exibição de alertas para o usuário, facilitando os testes.
- **IMainThreadInvoker / RealMainThreadInvoker.cs:** Uma abstração crucial que encapsula a chamada ao `MainThread`. Isso permite que a ViewModel seja testada unitariamente sem depender diretamente de uma thread de UI, que não existe em um ambiente de teste.

## Fluxo de Dados (Ao clicar em "Rodar Simulação")

1.  A `View` (`BrownianMotionPage`) aciona o `StartSimulationCommand` na `ViewModel`.
2.  A `ViewModel` entra no estado `IsBusy = true`, mostrando o indicador de atividade.
3.  Os dados de entrada são validados. Se forem inválidos, o `IDialogService` exibe um alerta.
4.  Se os dados forem válidos, a `ViewModel` cria um `BrownianDataModel`.
5.  A `ViewModel` chama o método `GenerateSimulation` do `IBrownianService` dentro de um `Task.Run`, passando os dados e um gerador `Random`. Isso executa o cálculo pesado em uma thread de segundo plano, mantendo a UI responsiva.
6.  Após o serviço retornar o array de `double[]`, a `ViewModel` usa o `IMainThreadInvoker` para despachar a criação do gráfico de volta para a thread principal da UI.
7.  O método `CreateChart` converte o array de `double` em `ChartEntry`, configura as propriedades visuais (cor, preenchimento, etc.) e atribui o novo `LineChart` à propriedade `SimulationChart`.
8.  A `View`, que está vinculada a `SimulationChart`, se atualiza automaticamente para exibir o novo gráfico.
9.  A `ViewModel` define `IsBusy = false`, escondendo o indicador de atividade.

#Projeto Teste.
executar o projeto teste (alterar para execução principal.)

---
