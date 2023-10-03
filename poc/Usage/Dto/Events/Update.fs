module Usage.Dto.Events.Update

open System

type UpdateDto =
    { Name: string
      OriginalValue: string option
      UpdatedDate: DateTime }
