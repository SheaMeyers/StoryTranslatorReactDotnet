import { useEffect, useState } from "react"
import { getBooks } from "../apis/BookApi"
import Select from "@mui/material/Select"
import MenuItem from "@mui/material/MenuItem"
import Popover from "@mui/material/Popover"
import Button from "@mui/material/Button"
import TextField from "@mui/material/TextField"
import { Paragraph } from "../types"
import { getFirstAndLastParagraphId, getFirstParagraph, getParagraph } from "../apis/ParagraphsApi"
import "../styling/Book.css"
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup"
import ToggleButton from "@mui/material/ToggleButton"
import { getFirstParagraphIdCookie, getLastParagraphIdCookie, getParagraphCookie, getSelectedBookCookie, getSelectedTranslateFromCookie, getSelectedTranslateToCookie, setFirstParagraphIdCookie, setLastParagraphIdCookie, setParagraphCookie, setSelectedBookCookie, setSelectedTranslateFromCookie, setSelectedTranslateToCookie } from "../cookies"


const Book = () => {

  const languages = ['English', 'Spanish', 'French', 'German']

  const [books, setBooks] = useState<string[]>([])
  
  const [selectedBook, setSelectedBook] = useState<string>(getSelectedBookCookie())
  const [translateFromSelector, setTranslateFromSelector] = useState<string>(getSelectedTranslateFromCookie())
  const [translateToSelector, setTranslateToSelector] = useState<string>(getSelectedTranslateToCookie())

  const [firstParagraphId, setFirstParagraphId] = useState<number>(getFirstParagraphIdCookie())
  const [lastParagraphId, setLastParagraphId] = useState<number>(getLastParagraphIdCookie())

  const [paragraph, setParagraph] = useState<Paragraph>(getParagraphCookie())

  const updateSelectedBook = (value: string) => {
    setSelectedBook(value)
    setSelectedBookCookie(value)
  }

  const updateTranslateFromSelector = (value: string) => {
    setTranslateFromSelector(value)
    setSelectedTranslateFromCookie(value)
  }

  const updateTranslateToSelector = (value: string) => {
    setTranslateToSelector(value)
    setSelectedTranslateToCookie(value)
  }

  const updateFirstParagraphId = (value: number) => {
    setFirstParagraphId(value)
    setFirstParagraphIdCookie(value)
  }

  const updateLastParagraphId = (value: number) => {
    setLastParagraphId(value)
    setLastParagraphIdCookie(value)
  }

  const updateParagraph = (value: Paragraph) => {
    setParagraph(value)
    setParagraphCookie(value)
  }

  const [isPopoverOpen, setIsPopoverOpen] = useState<boolean>(false)

  const [mode, setMode] = useState<"read" | "write">("read")

  const handleGetParagraph = async (id: number) => {
    const paragraph = await getParagraph(id, translateFromSelector, translateToSelector)
    updateParagraph(paragraph)
  }
  
  useEffect(() => {
    const fetchBooks = async () => {
      const retrievedBooks = await getBooks()
      setBooks(retrievedBooks)
    }
    fetchBooks()  
  }, [])

  useEffect(() => {
    const fetchFirstParagraph = async () => {
      const paragraph = await getFirstParagraph(selectedBook, translateFromSelector, translateToSelector)
      const {firstId, lastId} = await getFirstAndLastParagraphId(selectedBook)
      updateParagraph(paragraph)
      updateFirstParagraphId(firstId)
      updateLastParagraphId(lastId)
    }
    if (selectedBook && translateFromSelector && translateToSelector && paragraph.id === -1) {
      fetchFirstParagraph()
    }
  }, [selectedBook, translateFromSelector, translateToSelector, paragraph])

  return (
    <>
      <div className="BookSelectorBar">
        <Select
          labelId="select-book-label"
          id="select-book"
          value={selectedBook}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => updateSelectedBook(e.target.value)}
        >
          {books.map(book => <MenuItem value={book}>{book}</MenuItem>)}
        </Select>
        <Select
          labelId="translate-from-label"
          id="translate-from"
          value={translateFromSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => updateTranslateFromSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
        <Select
          labelId="translate-to-label"
          id="translate-to"
          value={translateToSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => updateTranslateToSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
      </div>
      {selectedBook && translateFromSelector && translateToSelector ?
        <div className="TranslationsContainer">
        {/* <ToggleButtonGroup
          color="primary"
          value={mode}
          exclusive
          onChange={(_e, newValue: "read" | "write") => setMode(newValue)}
          aria-label="Mode"
          className="ToggleButtonGroup"
        >
          <ToggleButton value="read">Read</ToggleButton>
          <ToggleButton value="write">Write</ToggleButton>
        </ToggleButtonGroup> */}
          <div className="TranslationsTextContainer">
            <TextField
              id="translate-from-text"
              label="Multiline"
              className="TranslationText"
              multiline
              rows={15}
              value={paragraph.translateFrom}
              onClick={() => setIsPopoverOpen(true)}
            />
            <Popover
              id='translate-to-text'
              open={isPopoverOpen}
              anchorEl={document.getElementById('translate-from-text')}
              onClose={() => setIsPopoverOpen(false)}
              anchorOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
              transformOrigin={{
                vertical: "bottom",
                horizontal: "left",
              }}
            >
              {paragraph.translateTo}
            </Popover>
            {mode === "write" && 
              <TextField
                id="translate-to-text"
                label="Multiline"
                className="TranslationText"
                multiline
                rows={15}
                // value={paragraph.translateFrom}
                // onClick={() => setIsPopoverOpen(true)}
              />
            }
          </div>
          <div className="ButtonContainer">
            <Button 
              variant="contained" 
              onClick={() => handleGetParagraph(paragraph.id - 1)}
              disabled={paragraph.id === firstParagraphId}
            >
              Previous
            </Button>
            <Button 
              variant="contained" 
              onClick={() => handleGetParagraph(paragraph.id + 1)}
              disabled={paragraph.id === lastParagraphId}
            >
              Next
            </Button>
          </div>
        </div>
      :
        <p className="ChooseBookText">Choose book and languages from dropdowns to begin</p>
      }
    </>
  )
}

export default Book
