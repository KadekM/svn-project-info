module CommandLine

(*
-ssr / --set-svn-repository - svn repository

*)

type SvnRepositoryOption = | SvnRepositoryOption of string

type CommandLineOptions = {
    svn: SvnRepositoryOption;
}


let defaultCommandLineOptions = { svn = SvnRepositoryOption("none") }

let (|Prefix|_|) (p: string) (s: string) =
    if s.StartsWith(p) then Some(s.Substring(p.Length))
    else None

let rec parseCommandLineRec args options =
    match args with
    | x::xs ->
        match x with
        | Prefix "-ssr=" url | Prefix "--set-svn-repository=" url -> {options with svn=SvnRepositoryOption(url)} |> parseCommandLineRec xs
        | arg -> printfn "Argument %s is not known" arg
                 options |> parseCommandLineRec xs
    | _ -> options

        
        