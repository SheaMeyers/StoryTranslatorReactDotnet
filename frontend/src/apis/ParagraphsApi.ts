import { Paragraph } from "../types";


export const getFirstParagraph = async (bookTitle: string, translateFrom: string, translateTo: string): Promise<Paragraph> => {
    const response = await fetch(`/paragraph/GetFirstParagraph/${bookTitle}/${translateFrom}/${translateTo}`, { method: 'GET'})
    return await response.json();
}
