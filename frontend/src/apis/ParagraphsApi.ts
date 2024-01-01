import { Paragraph } from "../types";


export const getFirstParagraph = async (bookTitle: string, translateFrom: string, translateTo: string): Promise<Paragraph> => {
    const response = await fetch(`/paragraph/GetFirstParagraph/${bookTitle}/${translateFrom}/${translateTo}`, { method: 'GET'})
    return await response.json();
}

export const getFirstAndLastParagraphId = async (bookTitle: string): Promise<{firstId: number, lastId: number}> => {
    const response = await fetch(`/paragraph/GetFirstAndLastParagraphId/${bookTitle}`, { method: 'GET'})
    return await response.json();
}

export const getParagraph = async (id: number, TranslateFrom: string, TranslateTo: string): Promise<Paragraph> => {
    const response = await fetch(`/paragraph/${id}`, { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },      
        body: JSON.stringify({ TranslateFrom, TranslateTo })
    })
    
    return await response.json();
}
