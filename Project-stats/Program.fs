open FSharp.Data;

type LogProvider = XmlProvider<"sample-log.xml">
let provider path = LogProvider.Parse(path) 


[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let log = LogProvider.Load "log.xml"

    let path = System.Environment.GetEnvironmentVariable("PATH").Split(';') |> Seq.map(fun x -> System.IO.Path.Combine(x, "svn.exe")) |> Seq.where System.IO.File.Exists|> Seq.head
    let procInfo = System.Diagnostics.ProcessStartInfo(path, "--xml")
    
    System.Diagnostics.Process.Start(procInfo)

    let commitsFrom author = log.Logentries |> Seq.where (fun x -> x.Author = author)

    let authors = [| "marek.kadek"; "martin.kolinek"; "vladimir.pavelka"; "marek.sedlacek"; "robert.herceg"; "branislav.pavelka" |]

    authors |> Seq.map (fun x -> (x, commitsFrom x |> Seq.length ) ) |> Seq.iter (fun (author, commits) -> printfn "%A : %A" author commits)
    System.Console.ReadKey() |> ignore
    0
