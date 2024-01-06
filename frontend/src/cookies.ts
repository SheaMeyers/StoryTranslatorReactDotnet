// setCookie and getCookie code largely taken from W3 Schools https://www.w3schools.com/js/js_cookies.asp

import { Paragraph } from "./types";

const setCookie = (cookieName: string, cookieValue: string, days: number = 365): void => {
    const d = new Date();
    d.setTime(d.getTime() + (days*24*60*60*1000));
    let expires = "expires="+ d.toUTCString();
    document.cookie = cookieName + "=" + cookieValue + ";" + expires + ";path=/";
}

const getCookie = (cname: string): string => {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for(let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
}

const selectedBookCookieName = 'selectedBookCookie'
export const setSelectedBookCookie = (value: string): void => setCookie(selectedBookCookieName, value)
export const getSelectedBookCookie = (): string => getCookie(selectedBookCookieName)

const selectedTranslateFromCookieName = 'selectedTranslateFromCookie'
export const setSelectedTranslateFromCookie = (value: string): void => setCookie(selectedTranslateFromCookieName, value)
export const getSelectedTranslateFromCookie = (): string => getCookie(selectedTranslateFromCookieName)

const selectedTranslateToCookieName = 'selectedTranslateToCookie'
export const setSelectedTranslateToCookie = (value: string): void => setCookie(selectedTranslateToCookieName, value)
export const getSelectedTranslateToCookie = (): string => getCookie(selectedTranslateToCookieName)

const firstParagraphIdCookieName = 'firstParagraphIdCookie'
export const setFirstParagraphIdCookie = (value: number): void => setCookie(firstParagraphIdCookieName, value.toString())
export const getFirstParagraphIdCookie = (): number => parseInt(getCookie(firstParagraphIdCookieName) || "-1")

const lastParagraphIdCookieName = 'lastParagraphIdCookie'
export const setLastParagraphIdCookie = (value: number): void => setCookie(lastParagraphIdCookieName, value.toString())
export const getLastParagraphIdCookie = (): number => parseInt(getCookie(lastParagraphIdCookieName) || "-1")

const paragraphCookieName = 'paragraphCookie'
export const setParagraphCookie = (value: Paragraph): void => setCookie(paragraphCookieName, JSON.stringify(value))
export const getParagraphCookie = (): Paragraph => {
    var value = getCookie(paragraphCookieName)
    if (value) return JSON.parse(value)

    return {
        id: -1,
        translateFrom: '',
        translateTo: ''
    }
}

const userTranslationCookieName = 'userTranslationCookie'
export const setuserTranslationCookie = (value: string): void => setCookie(userTranslationCookieName, value)
export const getuserTranslationCookie = (): string => getCookie(userTranslationCookieName)
