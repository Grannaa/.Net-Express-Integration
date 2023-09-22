'use strict';
var express = require('express');
var router = express.Router();
var fs = require('fs');
const { Server } = require('https');
//Cuento la cantidad de ficheros para que si se borra una nota de entre medias entre la nota 1 y la n, siga mirando notas en base a la cantidad que hay y que no pare
//porque no existen de forma consecutiva
var cont_fichero = fs.readdirSync('./').filter(fn => fn.startsWith('nota_')).length;
/* GET home page. */
router.get('/', function (req, res) {
    res.render('index', { title: 'Express' });
    let i = 0;
});

router.post('/', function (req, res) {
    let i = 0;
    let nombre_fichero = "./nota_" + i + ".txt";
    console.log(nombre_fichero);
    while (fs.existsSync(nombre_fichero)){
        i++;
        console.log(nombre_fichero + "existe");
        nombre_fichero = "./nota_" + i + ".txt";
    }
    fs.writeFileSync("./nota_" + i + ".txt", req.body.nota);
    cont_fichero++;
    serverLog("Agregada nota numero " + i + " Con texto: " + req.body.nota);
    res.send("Fichero Guardado!");    
});

router.get('/leer', function (req, res) {
    let dict = {};
    let i = 0;
    let nombre_fichero = "./nota_" + i + ".txt";
    while (i <= cont_fichero) {
        if (fs.existsSync(nombre_fichero)) {
            let nota = fs.readFileSync(nombre_fichero);
            dict["Nota " + i] = nota.toString();
        }
        i++;
        nombre_fichero = "./nota_" + i + ".txt";
    }
    serverLog("Lectura ");
    res.send(JSON.stringify(dict));
});

router.delete('/delete', function (req, res) {
    let i = 0;
    let nombre_fichero = "./nota_" + i + ".txt";;
    while (i < cont_fichero) {
        if (fs.existsSync(nombre_fichero)) {
            fs.unlinkSync(nombre_fichero)
            serverLog("Borrada nota numero " + i);
            cont_fichero--;
        }
        
        i++;
        nombre_fichero = "./nota_" + i + ".txt";
    }
    res.send("Notas Borradas");
    
})
function serverLog(operacion) {
    fs.appendFileSync("./server.log", operacion + new Date().toUTCString() + "\n");
}

module.exports = router;
