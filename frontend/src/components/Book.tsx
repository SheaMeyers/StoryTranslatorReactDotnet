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


const Book = () => {

  const languages = ['English', 'Spanish', 'French', 'German']

  const [books, setBooks] = useState<string[]>([])
  
  const [selectedBook, setSelectedBook] = useState<string>('')
  const [translateFromSelector, setTranslateFromSelector] = useState<string>('')
  const [translateToSelector, setTranslateToSelector] = useState<string>('')

  const [firstParagraphId, setFirstParagraphId] = useState<number>(-1)
  const [lastParagraphId, setLastParagraphId] = useState<number>(-1)
  const [paragraph, setParagraph] = useState<Paragraph>({
    id: -1,
    translateFrom: '',
    translateTo: ''
  })

  const [isPopoverOpen, setIsPopoverOpen] = useState<boolean>(false)

  const [mode, setMode] = useState<"read" | "write">("read")

  const handleGetParagraph = async (id: number) => {
    const nextParagraph = await getParagraph(id, translateFromSelector, translateToSelector)
    setParagraph({ ...nextParagraph })
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
      setParagraph({ ...paragraph })
      setFirstParagraphId(firstId)
      setLastParagraphId(lastId)
    }
    if (selectedBook && translateFromSelector && translateToSelector) {
      fetchFirstParagraph()
    }
  }, [selectedBook, translateFromSelector, translateToSelector])


  return (
    <>
      <div className="BookSelectorBar">
        <Select
          labelId="select-book-label"
          id="select-book"
          value={selectedBook}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => setSelectedBook(e.target.value)}
        >
          {books.map(book => <MenuItem value={book}>{book}</MenuItem>)}
        </Select>
        <Select
          labelId="translate-from-label"
          id="translate-from"
          value={translateFromSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => setTranslateFromSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
        <Select
          labelId="translate-to-label"
          id="translate-to"
          value={translateToSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => setTranslateToSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
      </div>
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
    </>
  )
}

export default Book
