<div class="mermaid">
    %%{ init : { 
      "theme" : "base",
      'themeVariables': {
        'lineColor': 'white'},
      "flowchart" : { "curve" : "linear"}
    }}%%

    flowchart TD;

    %% Subgraph for Main Form
    subgraph A[Main Form]
        B[Data View Import Data]:::frontStyle
        C[Data View DB Data]:::frontStyle
        D[CSV Import Module]:::toolStyle
        E[Validation Module]:::backStyle
        F[Filtering Module]:::toolStyle
        H[Export Module]:::toolStyle
    end

    A:::subStyle

    G[Database Module]:::backStyle
    I[Purge]:::backStyle
    J[(Azure Cloud Database)]:::cloudStyle
    K[(Local Storage)]:::dbStyle

    %% Define connections with actions
    K -->|Import CSV| D
    D -->|Parse Data| B
    D -->|Parse Data| E
    G --->|Pull Data| E & C
    J -->|Pull Data| G

    G --> I
    
    E -->|Validate and Compare| F & B
    F --->|Filter Data| G & B
    F -->|Filter Data| H
    
    H --->|Export CSV| K
    
    I -->|Purge DB| J
    G -->|Push Data| J

    %% Apply custom styles
    classDef subStyle fill:darkgrey, rx:15, ry:15, stroke:black, stroke-width:3px, font-weight:bold;
    classDef cloudStyle fill:cyan, color:black, stroke:black, stroke-width:3px, font-weight:bold;
    classDef backStyle fill:darkblue, color:white, rx:15, ry:15, stroke:black, stroke-width:3px, font-weight:bold;
    classDef frontStyle fill:coral, color:black, rx:15, ry:15, stroke:black, stroke-width:3px, font-weight:bold;
    classDef toolStyle fill:yellow, color:black, stroke:black, stroke-width:3px, font-weight:bold;
    classDef dbStyle fill:orange, color:black, stroke:black, stroke-width:3px, font-weight:bold;
</div>
