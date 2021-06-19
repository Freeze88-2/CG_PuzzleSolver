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
- [Métodos](#métodos)
- [Outras Soluções](#outras-soluções)
- [Problemas](#problemas)

### Algoritmo

O algorítmo completo que resolve o puzzle encontra-se na seguinte função dentro da `ImageClass`:

`Image<Bgr, byte> puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)`

Para esta solução encontramos peças independentes dentro de uma imagem, criamos novas imagens por peça e por fim encontramos pares e juntamos até ficarmos com apenas 1 peça. Tentámos de outras formas antes de chegarmos a esta solução e a descrição dessas e porque é que falharam estarão na secção [Outras Soluções](#outras-soluções)

#### Descrição

* Começamos por preparar os apontadores necessários para ler a imagem e criamos um vetor de `PuzzlePiece`, classe que contém toda a informação necessária para uma peça e as funções que comparam e juntam estas.

* Este vetor é preenchido pela função `DetectIndependentObjects` da `ImageClass` que usa o algorítmo de objetos ligados para contar e separar cada peça pelos seus limites, criando posições (x,y) para topo esquerdo e canto direito de cada peça.

* O vetor das imagens é passado para outra função `FindRotation` que calcula e tenta rodar cada peça em volta do centro da própria. Esta rotação é escrita para a imagem original, ficando assim com uma imagem de peças direitas e um vetor de peças de puzzle com os limites definidos.

* Com esta informação, inicializamos cada `PuzzlePiece` chamando `CreateImage(Image<Bgr, Byte> original)` para cada peça, esta função cria e guarda uma imagem que contem únicamente a peça correspondente aos limites criados anteriormente.

* Com todas as peças base inicializadas, corremos o algoritmo da função `PuzzlePiece Join(List<PuzzlePiece> pieces)` que retorna uma única peça final de posição 0,0 e com comprimento e largura do nosso puzzle resolvido, sendo apenas necessário usar a propriedade `Img` de `PuzzlePiece` para retornar a imagem recortada final.

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

Por fim, por cada lado, percorrer todas as peças, ignorando a atual, calculando a distância entre as duas pelo lado, feito através da função `Compare(PuzzlePiece a, PuzzlePiece b, Side side)` da classe `PuzzlePiece`. Esta função retorna a soma de todas as distâncias `RGB` entre lados adjacentes de a e b, se a e b tiverem o lado a comparar de tamanha diferente, esta função retorna imediatamente `PositiveInfinity`.

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

### Métodos



### Outras Soluções



### Problemas



[André Vitorino]: https://github.com/Freeze88-2
[Rodrigo Pinheiro]: https://github.com/RodrigoPrinheiro
[bounds]: images/bounds.png