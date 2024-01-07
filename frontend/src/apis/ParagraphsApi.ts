import { Paragraph } from "../types";


export const getFirstParagraph = async (bookTitle: string, translateFrom: string, translateTo: string): Promise<Paragraph & {firstId: number, lastId: number}> => {
    const response = await fetch(`/paragraph/GetFirstParagraph/${bookTitle}/${translateFrom}/${translateTo}`, { method: 'GET'})
    return await response.json();
}

export const getParagraph = async (CurrentId: number, Change: number, TranslateFrom: string, TranslateTo: string, UserTranslation: string, apiToken: string): Promise<Paragraph & { userTranslation: string, apiToken: string}> => {
    const response = await fetch('/paragraph', { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': apiToken
          },      
        body: JSON.stringify({ CurrentId, Change, TranslateFrom, TranslateTo, UserTranslation })
    })
    
    var json = await response.json()

    debugger

    return json
}
