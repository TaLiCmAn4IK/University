const EventEmitter = require('events');

class DB extends EventEmitter {
    constructor() {
        super();
        this.rows = [
            { id: 1, name: 'Иван', bday: '2000-01-01' },
            { id: 2, name: 'Мария', bday: '1998-05-12' }
        ];
    }

    select() {
        this.emit('GET', this.rows);
        return this.rows;
    }

    insert(row) {
        row.id = this.rows.length ? Math.max(...this.rows.map(r => r.id)) + 1 : 1;
        this.rows.push(row);
        this.emit('POST', row);
        return row;
    }

    update(row) {
        const index = this.rows.findIndex(r => r.id === row.id);
        if (index !== -1) {
            this.rows[index] = { ...this.rows[index], ...row };
            this.emit('PUT', this.rows[index]);
            return this.rows[index];
        }
        return null;
    }

    delete(id) {
        const index = this.rows.findIndex(r => r.id === id);
        if (index !== -1) {
            const deleted = this.rows.splice(index, 1)[0];
            this.emit('DELETE', deleted);
            return deleted;
        }
        return null;
    }
}

module.exports = DB;