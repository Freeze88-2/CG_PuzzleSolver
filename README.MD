# CG Puzzle Solver

* [Repositório de código fonte](https://github.com/Freeze88-2/CG_PuzzleSolver)

## Autores

[André Vitorino] 21902633

[Rodrigo Pinheiro] 21802488

### Índice
- [Índice](#índice)
- [Algoritmo](#algoritmo)
  - [Descrição](#descrição)
  - [Função Join](#função-join)
- [Classes e Estruturas](#classes-e-estruturas)
  - [PuzzlePiece](#puzzlepiece)
  - [Vector2Int](#vector2int)
- [Métodos Extra](#métodos-extra)
- [Outras Soluções](#outras-soluções)
- [Problemas Atuais](#problemas-atuais)

### Algoritmo

O algorítmo completo que resolve o puzzle encontra-se na seguinte função dentro da `ImageClass`:

`Image<Bgr, byte> puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)`

Para esta solução encontramos peças independentes dentro de uma imagem, criamos novas imagens por peça e por fim encontramos pares e juntamos até ficarmos com apenas 1 peça. Tentámos de outras formas antes de chegarmos a esta solução e a descrição dessas e porque é que falharam estarão na secção [Outras Soluções](#outras-soluções)

#### Descrição

1. Começamos por preparar os apontadores necessários para ler a imagem e criamos um vetor de `PuzzlePiece`, classe que contém toda a informação necessária para uma peça e as funções que comparam e juntam estas.

2. Este vetor é preenchido pela função `DetectIndependentObjects` da `ImageClass` que usa o algorítmo de objetos ligados para contar e separar cada peça pelos seus limites, criando posições (x,y) para topo esquerdo e canto direito de cada peça.

3. O vetor das imagens é passado para outra função `FindRotation` que calcula e tenta rodar cada peça em volta do centro da própria. Esta rotação é escrita para a imagem original, ficando assim com uma imagem de peças direitas e um vetor de peças de puzzle com os limites definidos.

4. Com esta informação, inicializamos cada `PuzzlePiece` chamando `CreateImage(Image<Bgr, Byte> original)` para cada peça, esta função cria e guarda uma imagem que contem únicamente a peça correspondente aos limites criados anteriormente.

5. Com todas as peças base inicializadas, corremos o algoritmo da função `PuzzlePiece Join(List<PuzzlePiece> pieces)` que retorna uma única peça final de posição 0,0 e com comprimento e largura do nosso puzzle resolvido, sendo apenas necessário usar a propriedade `Img` de `PuzzlePiece` para retornar a imagem recortada final.

#### Função Join

Olhando primeiro para o corpo da função:

```cs
public static PuzzlePiece Join(List<PuzzlePiece> pieces)
{
    // while
    for (int i = 0; i < pieces.Count; i++)
    {
        Side connectionSide = Side.Top;
        double min = float.PositiveInfinity;
        int minIndex = -1
        for (int side = 0; side < 4; side++)
        {
            for (int j = 0; j < pieces.Count; j++)
            {
                // Protect from comparing with the same puzzle piece
                if (i == j) continue
                double dist = PuzzlePiece.Compare(pieces[i]pieces[j], (Side)side)
                if (dist < min)
                {
                    min = dist;
                    minIndex = j;
                    connectionSide = (Side)side;
                }
            }
        
        if (minIndex > -1)
        {
            // Combine
            pieces[i] = pieces[i].Combine(pieces[minIndex]connectionSide);
            pieces.Remove(pieces[minIndex]);
            i = 0;
        }
    }
    return pieces[0];
}
```

No ínicio da nossa função entramos num _for loop_ que percorre todas as peças da lista inicial, e, por cada peça, descobre uma peça par com mínimo de distância `RGB` entre lados adjacentes. Esta lógica está desconstruída em mais 2 _nested for loops_ um primeiro que percorre os lados possíveis com ajuda de uma enumeração `Side`

```cs
public enum Side
{
    Top = 0,    // Adjacent to Bottom
    Right = 1,  // Adjacent to Left
    Bottom = 2, // Adjacent to Top
    Left = 3    // Adjacent to Right
}
```

No segundo _loop_, por cada lado, percorrer todas as peças, ignorando a atual, calculando a distância entre as duas pelo lado, feito através da função `Compare(PuzzlePiece a, PuzzlePiece b, Side side)` da classe `PuzzlePiece`. Esta função retorna a soma de todas as distâncias `RGB` entre lados adjacentes de `a` e `b`, se `a` e `b` tiverem o lado a comparar de tamanho diferente, esta função retorna imediatamente `PositiveInfinity`, ignorando a possíbilidade de conceção.

Quando saímos dos 2 _nested loops_ anteriores, temos acesso a 2 peças e ao lado pelas quais estas devem ser combinadas, sendo apenas uma questão de as combinar usando o método `Combine` de `PuzzlePiece`. Este método por sua vez calcula a nova largura e comprimento das imagens e usa o método `Image<Bgr, byte>.CopyTo()` para criar a nova peça adequadamente.

Se a peça atual da procura for combinada com outra, reiniciamos o loop `i = 0;` de modo a verificar se as alguma peça que anteriormente não tenha tido par já tem agora que houve uma combinação.

### Classes e Estruturas

#### PuzzlePiece

Para a nossa solução final foi apenas criada a classe `PuzzlePiece` que contém várias funcionalidades e propriedades usadas no nosso algorítmo. O objétivo príncipal desta classe é ter a informação necessária por peça para ler os pixeis correspondentes da imagem original de onde a peça foi criada e também realizar operações sobre pares de peças.

`PuzzlePiece` contém um único construtor, este recebe o canto superior esquerdo e o canto inferior direito da peça em questão na forma de `Vector2Int`. Também é possível passar o ângulo da peça se este já tiver sido calculado.

![boundsImage][bounds]

```cs
public PuzzlePiece(Vector2Int top, Vector2Int bottom, double angle = 0)
{
    _top = top;
    _bottom = bottom
    _height = bottom.y - top.y + 1;
    _width = bottom.x - top.x + 1
    Angle = angle;
}
```
Propriedades

Limites da imagem, Topo e fundo
```cs
public Vector2Int Top => _top;
public Vector2Int Bottom => _bottom;
```

Altura e largura calculada no construtor
```cs
public int Height => _height;
public int Width => _width;
```

Ângulo da imagem
```cs
public double Angle { get; }
```

Imagem da peça criada no método `void CreateImage(Image<Bgr, byte> original)`
ou por combinação de peças

```cs
public Image<Bgr, byte> Img { get; private set; }
```

Métodos

`CreateImage` Cria a imagem da peça através da imagem completa original
```cs
public void CreateImage(Image<Bgr, byte> original)
{
    Img = new Image<Bgr, byte>(Width, Height);
    Rectangle cropArea = new Rectangle(Top.x, Top.y, Width, Height);
    Img.Bitmap = original.Bitmap.Clone(cropArea, original.Bitmap.PixelFormat);
}
```
`Combine` combina duas peças através do lado dado. Internamente usa o método `CopyTo()`

```cs
public PuzzlePiece Combine(PuzzlePiece other, Side side);
```

Devolve o ângulo de uma peça usando o topo e fundo da imagem
```cs
public static double ImageAngle(Vector2Int rightBottom, Vector2Int bottom)
{
    Vector2Int diagonal = rightBottom - bottom;

    double r = Math.Atan2(diagonal.y, diagonal.x);
    double angle = r * 180.0 / Math.PI;

    return -Math.Round(angle);
}
```

#### Vector2Int

Por preferência, decidimos estruturar as coordenadas das imagens nesta estrutura simples em vez de optar por `Tuples` ou pela estrutura `Point`.

```cs
internal struct Vector2Int
{
    public static Vector2Int Zero => new Vector2Int(0, 0);
    
    public int x;
    public int y;
    
    public Vector2Int(int x = default, int y = default)
    {
        this.x = x;
        this.y = y;
    }
    
    // Distance between 2 points
    public static double Distance(Vector2Int a, Vector2Int b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => newVector2Int(a.x + b.x, a.y + b.y);
    
    public static Vector2Int operator -(Vector2Int a, Vector2Int b) => newVector2Int(a.x - b.x, a.y - b.y);
}
```

Para simplicidade, adicionámos uma propriedade estática que retorna um novo vector a zeros, um método de distância e dois _overrides_ para os operadores `+` e `-` 

### Métodos Extra

Adicionámos vários métodos a `ImageClass` que nos ajudam a visualizar os passos do algoritmo de resolução de puzzles.

```cs
public unsafe static PuzzlePiece[] DetectIndependentObjects(byte* dataPtrWrite, byte* dataPtrRead, int nChan, int widthStep, int width, int height)
```

```cs
public static void DrawBounds(Image<Bgr,byte> img, Image<Bgr, byte> imgCopy)
```

```cs
public static void RotateIndividualPieces(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
```

### Outras Soluções

Até chegarmos à solução atual experimentámos de várias formas diferentes, sendo o mais difícil encontrar uma forma clara e simples de connecter e mover as peças para as suas posições finais assim como compara-las de forma a não ter que calcular as distâncias a cada iteração do algoritmo.

**Conectividade de peças**

Em vez de criarmos imagens separadas por peças, eram guardadas as posições de cada peça da mesma forma através da `PuzzlePiece`, no entanto quando as queremos combinar, aplicava-mos uma translação de acordo com o lado a connecter na imagem final, movendo os pixeis da imagem original para a sua posição final da imagem de escrita. Por fim após termos as peças nos locais corretos usávamos o método de clonagem de imagens com o parâmetro opcional da zona de clonagem, retirando apenas a peça final.

Problemas com a solução:
   * Peças, por vezes, podiam ser movidas para fora da imagem final, perdendo informação.
   * Algorítmos de comparação e junção mais lentos

Pontos fortes:
   * Uso de memória muito reduzido comparado com a solução atual.

**Distância e comparação de bordas**

Inicialmente tentamos uma abordagem diferente para comparar as bordas das peças. Cada peça, após ser construída, percorria as suas bordas por lado e guardava na sua instância a soma do valor dos pixeis desse lado. Quando queríamos comparar duas peças apenas necessitávamos de ver a diferença entre a soma dos lados correspondentes. Quando criávamos uma nova peça era apenas necessário somar os valores pré-calculados de acordo com os lados que união.

Problemas:
   * Raramente duas peças poderiam juntar-se pelo lado errado

Pontos fortes:
   * Retira bastante tempo ao algoritmo e usa valores antigos para calcular novos

### Problemas Atuais

1. De momento não é possível resolver imagens com peças rodadas, o programa consegue identificar e rodar peças mas estas ficam com desvios que afetam os passos de conexão do algoritmo. O utilizador pode usar o item de menu `Rotate Individual Pieces` para ver o resultado desta operação.

[André Vitorino]: https://github.com/Freeze88-2
[Rodrigo Pinheiro]: https://github.com/RodrigoPrinheiro
[bounds]: images/bounds.png
[functions]: images/functionsCG.png